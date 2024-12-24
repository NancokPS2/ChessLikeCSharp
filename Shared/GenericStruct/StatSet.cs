using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Xml;
using ChessLike.Entity;
using Godot;

namespace ChessLike.Shared;
public partial class StatSet<TStatEnum> where TStatEnum : notnull, Enum
{
    public const string INVALID_BOOST_SOURCE = "__INVALID__";
    public delegate void StatChange(TStatEnum name, float amount);
    public event StatChange StatValueChanged;

    public Dictionary<TStatEnum, float> MaxDict {get; set;} = new();
    public Dictionary<TStatEnum, float> CurrentDict {get; set;} = new();
    public Dictionary<string, StatBoost> Boosts = new();

    public StatSet()
    {
        foreach (TStatEnum stat in Enum.GetValues(typeof(TStatEnum)))
        {
            MaxDict[stat] = new();
        }
    }

    public StatSet(Dictionary<TStatEnum, float> values) : base()
    {
        foreach (TStatEnum stat_name in values.Keys)
        {
            SetStat(stat_name, values[stat_name]);
        }
    }


    public float ChangeValue(TStatEnum stat, float amount)
    {
        float original_value = GetValue(stat);
        CurrentDict[stat] = CurrentDict[stat] + amount;
        float final_value = GetValue(stat);
        return final_value - original_value;
    }


    public void SetValue(TStatEnum stat, float value)
    {
        float original_val;
        CurrentDict.TryGetValue(stat, out original_val);
        float max = GetMax(stat);
        CurrentDict[stat] = MathF.Min(value, max);

        StatValueChanged?.Invoke(stat, value - original_val);
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
        MaxDict[stat] = MaxDict[stat] + value;
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
            SetStat(stat, GetMax(stat));
        }
    }

    private StatBoost BoostGetInstance(string source)
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
        if (booster.GetBoostSource() == INVALID_BOOST_SOURCE){throw new Exception("Invalid source.");}
        StatBoost? boost = booster.GetStatBoost();
        if (boost is not null)
        {
            BoostAdd(booster.GetBoostSource(), boost);
        }
    }

    private void BoostAdd(string source, StatBoost boost, bool replace = true)
    {
        //Replacing the boost with a new one. Force a replacement if there is no source in the first place.
        if (replace || !Boosts.ContainsKey(source))
        {
            Boosts[source] = boost;
        }
        //Not replacing and there is an existing boost, add to it.
        else
        {
            Boosts[source] = Boosts[source] + boost;
        }

    }


    public void BoostRemove(string source)
    {
        if (source == INVALID_BOOST_SOURCE){throw new Exception("Invalid source.");}
        if (Boosts.ContainsKey(source))
        {
            Boosts.Remove(source);
        }
    }
/*     public void BoostReset(string source)
    {
        Boosts[source] = new(source);
    }

    public void BoostSetMaxAdditive(string source, TStatEnum stat, float amount)
    {
        BoostGetInstance(source).MaxAdditiveBonus[stat] = amount;
    }

    public void BoostSetValueAdditive(string source, TStatEnum stat, float amount)
    {
        BoostGetInstance(source).MaxAdditiveBonus[stat] = amount;
    }

    public void BoostSetMaxMultiplicative(string source, TStatEnum stat, float amount)
    {
        BoostGetInstance(source).MaxMultiplicativeBonus[stat] = amount;
    }

    public void BoostSetValueMultiplicative(string source, TStatEnum stat, float amount)
    {
        BoostGetInstance(source).ValueMultiplicativeBonus[stat] = amount;
    } */

    public static TStatEnum[] GetArrayOfNames()
    {
        int total_stats = Enum.GetNames(typeof(TStatEnum)).Length;
        TStatEnum[] output = new TStatEnum[total_stats];

        int index = 0;
        foreach (TStatEnum name in Enum.GetValues(typeof(TStatEnum)))
        {
            output[index] = name;
            index ++;
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


    public class StatBoost
    {
        public string Source;
        //public Dictionary<TStatEnum, float> ValueAdditiveBonus = new();
        //public Dictionary<TStatEnum, float> ValueMultiplicativeBonus = new();
        private Dictionary<TStatEnum, float> MaxAdditiveBonus = new();
        private Dictionary<TStatEnum, float> MaxMultiplicativeBonus = new();

        public StatBoost(string Source)
        {
            this.Source = Source;
        }
/* 
        public float GetAdditiveValue(TStatEnum stat) => 
            ValueAdditiveBonus.ContainsKey(stat) ? ValueAdditiveBonus[stat] : 0;

        public float GetMultiplicativeValue(TStatEnum stat) => 
            ValueMultiplicativeBonus.ContainsKey(stat) ? ValueMultiplicativeBonus[stat] : 1;
 */
        public float GetAdditiveMax(TStatEnum stat) => 
            MaxAdditiveBonus.ContainsKey(stat) ? MaxAdditiveBonus[stat] : 0;

        public float GetMultiplicativeMax(TStatEnum stat) => 
            MaxMultiplicativeBonus.ContainsKey(stat) ? MaxMultiplicativeBonus[stat] : 1;
        
/* 
        public void SetAdditiveValue(TStatEnum stat, float value)
        {
            ValueAdditiveBonus[stat] = value;
        }

        public void SetMultiplicativeValue(TStatEnum stat, float value)
        {
            ValueMultiplicativeBonus[stat] = value;
        }
 */
        public void SetAdditiveMax(TStatEnum stat, float value)
        {
            MaxAdditiveBonus[stat] = value;
        }

        public void SetMultiplicativeMax(TStatEnum stat, float value)
        {
            MaxMultiplicativeBonus[stat] = value;
        }

        public static StatBoost operator+(StatBoost sourcer, StatBoost added)
        {
            StatBoost output = new(sourcer.Source);
            if (sourcer.Source != added.Source)
            {
                throw new Exception("Differing Source properties, can't handle.");
            }
            foreach (TStatEnum item in Enum.GetValues(typeof(TStatEnum)))
            {
                float sourcer_max_add = sourcer.GetAdditiveMax(item);
                float added_max_add = added.GetAdditiveMax(item);

                float sourcer_max_mult = sourcer.GetMultiplicativeMax(item);
                float added_max_mult = added.GetMultiplicativeMax(item);

                output.SetAdditiveMax(item, sourcer_max_add + added_max_add);
                output.SetMultiplicativeMax(item, sourcer_max_mult + added_max_mult);
            }

            return output;
        }
    }

    public interface IStatBooster
    {
        public string GetBoostSource();
        public StatBoost? GetStatBoost();
    }
}

