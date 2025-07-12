using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Xml;
using ChessLike.Entity;
using Godot;

namespace ChessLike.Shared;
public partial class StatSet<[MustBeVariant]TStatEnum> : Resource where TStatEnum : notnull, Enum
{
    public readonly TStatEnum[] AllStats;

    public const string INVALID_BOOST_SOURCE = "__INVALID__";
    public delegate void StatChange(TStatEnum name, float amount);
    public event StatChange? StatValueChanged;

    public Dictionary<TStatEnum, float> MaxDict { get; set; } = new();
    public Dictionary<TStatEnum, float> CurrentDict { get; set; } = new();
    public Dictionary<string, StatBoost<TStatEnum>> Boosts = new();

    public StatSet()
    {
        AllStats = ((TStatEnum[])Enum.GetValues(typeof(TStatEnum))).Where(x => IsValidStat(x)).ToArray();

        foreach (TStatEnum stat in AllStats)
        {
            MaxDict[stat] = new();
            CurrentDict[stat] = new();
        }
    }

    protected virtual bool IsValidStat(TStatEnum stat) => true;

    public StatSet(Dictionary<TStatEnum, float> values) : this()
    {
        foreach (TStatEnum stat_name in values.Keys)
        {
            SetStat(stat_name, values[stat_name]);
        }
    }

    #region Modify stats
    public float ChangeValue(TStatEnum stat, float amount)
    {
        float original_value = GetValue(stat);
        SetValue(stat, GetValue(stat) + amount);
        float final_value = GetValue(stat);
        return final_value - original_value;
    }


    public void SetValue(TStatEnum stat, float value)
    {
        float original_val;
        CurrentDict.TryGetValue(stat, out original_val);
        float max = GetMax(stat);
        CurrentDict[stat] = MathF.Min(value, max);
    }

    public void SetValuePercent(TStatEnum stat, float percent)
    {
        if (percent > 1.0f || percent < 0.0f)
        {
            throw new ArgumentOutOfRangeException("Must be a float from 0 to 1.");
        }
        SetValue(stat, GetMax(stat) * percent);
    }

    public void ChangeMax(TStatEnum stat, float value)
    {
        SetMax(stat, GetMax(stat) + value);
    }

    public void SetMax(TStatEnum stat, float value)
    {
        MaxDict[stat] = value;
    }

    public float GetValue(TStatEnum stat)
    {
        var val = CurrentDict[stat];
        float additive = 0;
        float multiplicative = 1;
        /*      foreach (var item in Boosts.Values)
                {
                    additive += item.GetAdditiveValue(stat);
                    multiplicative *= item.GetMultiplicativeValue(stat);
                } 
        */
        Debug.Assert(multiplicative != 0);
        return Mathf.Snapped(
            MathF.Min(
                (val + additive) * multiplicative,
                GetMax(stat)),
            0.1f
            );
    }

    public float GetValuePrecent(TStatEnum stat)
    {
        return GetValue(stat) / GetMax(stat);
    }

    public float GetMax(TStatEnum stat)
    {
        float val;
        MaxDict.TryGetValue(stat, out val);
        float additive = 0;
        float multiplicative = 1;
        foreach (var item in Boosts.Values)
        {
            additive += item.GetAdditiveMax(stat);
            multiplicative *= item.GetMultiplicativeMax(stat);
        }
        Debug.Assert(multiplicative != 0);
        return Mathf.Snapped((val + additive) * multiplicative, 0.1f);
    }


    /// <summary>
    /// Set max and current, for stats that are not meant to not use the max value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value"></param>
    public void SetStat(TStatEnum stat, float value)
    {
        SetMax(stat, value);
        SetValue(stat, GetMax(stat));
        Debug.Assert(GetValue(stat) == GetMax(stat), "The value does not match.");
    }

    public void MultiplyStat(TStatEnum stat, float multiplier)
    {
        float current = GetValue(stat);
        SetStat(stat, current * multiplier);
    }

    public void MultiplyStat(TStatEnum stat, double multiplier)
    {
        MultiplyStat(stat, (float)multiplier);
    }

    /// <summary>
    /// Resets the specified stats to their maximum value. If none are specified, all of them are reset.
    /// </summary>
    /// <param name="stats"></param>
    public void SetToMax(TStatEnum[]? stats = null)
    {
        //Set ALL stats if null.
        stats ??= GetArrayOfNames();

        foreach (TStatEnum stat in stats)
        {
            SetValue(stat, GetMax(stat));
        }
    }
    #endregion

    #region Boosts
    public string BoostGetListOfStatChanges(TStatEnum name)
    {
        string output = $"{name} Boost \nBase: {MaxDict[name]}\n";

        foreach (StatBoost<TStatEnum> boost in Boosts.Values)
        {
            string source = boost.Source;
            float total_mult = boost.GetMultiplicativeMax(name);
            float total_add = boost.GetAdditiveMax(name);
            output += $"{source}: +{total_add} | (x{total_mult})\n";
        }
        return output;
    }

    public string BoostGetListOfStatChanges()
    {
        string output = "";
        foreach (var item in AllStats)
        {
            output += BoostGetListOfStatChanges(item) + "\n";
        }
        return output;
    }

    private StatBoost<TStatEnum> BoostGetFromSource(string source)
    {
        if (Boosts.ContainsKey(source))
        {
            return Boosts[source];
        }
        else
        {
            return new(source);
        }
    }

    public void BoostAdd(IStatBooster booster, bool replace)
    {
        if (booster.GetBoostSource() == INVALID_BOOST_SOURCE) { throw new Exception("Invalid source."); }
        StatBoost<TStatEnum>? boost = booster.GetStatBoost();
        if (boost is not null)
        {
            BoostAdd(boost);
        }
    }

    public void BoostAdd(StatBoost<TStatEnum> boost, bool replace = true)
    {
        string source = boost.Source;
        //Replacing the boost with a new one. Force a replacement if there is no source in the first place.
        if (replace || !Boosts.ContainsKey(source))
        {
            Boosts[source] = boost;
        }
        //Not replacing and there is an existing boost, add to it.
        else
        {
            Boosts[source] = StatBoost<TStatEnum>.Combined(Boosts[source], boost);
        }

    }


    public void BoostRemove(string source)
    {
        if (source == INVALID_BOOST_SOURCE) { throw new Exception("Invalid source."); }
        if (Boosts.ContainsKey(source))
        {
            Boosts.Remove(source);
        }
    }

    #endregion

    public static TStatEnum[] GetArrayOfNames()
    {
        int total_stats = Enum.GetNames(typeof(TStatEnum)).Length;
        TStatEnum[] output = new TStatEnum[total_stats];

        int index = 0;
        foreach (TStatEnum name in Enum.GetValues(typeof(TStatEnum)))
        {
            output[index] = name;
            index++;
        }
        return output;
    }
    public Dictionary<TStatEnum, float> GetMaxStatDictionary()
    {
        return MaxDict;
    }

    public static StatSet<TStatEnum> GetAverage(StatSet<TStatEnum> a, StatSet<TStatEnum> b)
    {
        StatSet<TStatEnum> output = new StatSet<TStatEnum>();

        foreach (TStatEnum stat in GetArrayOfNames())
        {
            float average_max = (a.GetMax(stat) + b.GetMax(stat)) / 2;
            float average_value = (a.GetValue(stat) + b.GetValue(stat)) / 2;

            output.SetMax(stat, average_max);
            output.SetValue(stat, average_value);
        }

        return output;
    }


    public interface IStatBooster
    {
        public string GetBoostSource();
        public StatBoost<TStatEnum>? GetStatBoost();
    }
}

