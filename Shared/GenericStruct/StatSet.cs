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

    public Dictionary<TStatEnum, ClampFloat> Contents {get; set;} = new();
    public Dictionary<string, StatBoost> Boosts = new();

    public StatSet()
    {
        foreach (TStatEnum stat in Enum.GetValues(typeof(TStatEnum)))
        {
            Contents[stat] = new();
        }
    }

    public StatSet(Dictionary<TStatEnum, float> values) : base()
    {
        foreach (TStatEnum stat_name in values.Keys)
        {
            SetStat(stat_name, values[stat_name]);
        }
    }


    public void ChangeValue(TStatEnum stat, float amount)
    {
        Contents[stat].ChangeValue(amount);
    }


    public void SetValue(TStatEnum stat, float value)
    {
        float original_val = Contents[stat].GetCurrent();
        Contents[stat].SetCurrent(value);

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
        Contents[stat].ChangeMax(value);
    }

    public void SetMax(TStatEnum stat, float value)
    {
        Contents[stat].SetMax(value);
    }

    public void SetMin(TStatEnum stat, float value)
    {
        if(value < 0){throw new ArgumentOutOfRangeException("Negative minimums are not supported yet.");}
        Contents[stat].SetMin(value);
    }

    public float GetValue(TStatEnum stat)
    {
        var val = Contents[stat].GetCurrent();
        float additive = 0;
        float multiplicative = 1;
/*      foreach (var item in Boosts.Values)
        {
            additive += item.GetAdditiveValue(stat);
            multiplicative *= item.GetMultiplicativeValue(stat);
        } 
*/
        Debug.Assert(multiplicative != 0);
        return (val + additive) * multiplicative;
    }

    public float GetValuePrecent(TStatEnum stat)
    {
        return GetValue(stat) / GetMax(stat);
    }

    public float GetMax(TStatEnum stat)
    {
        var val = Contents[stat].GetMax();
        float additive = 0;
        float multiplicative = 1;
        foreach (var item in Boosts.Values)
        {
            additive += item.GetAdditiveMax(stat);
            multiplicative *= item.GetMultiplicativeMax(stat);
        }
        Debug.Assert(multiplicative != 0);
        return (val + additive) * multiplicative;
    }

    public float GetMin(TStatEnum stat)
    {
        return Contents[stat].GetMin();
    }


    /// <summary>
    /// Set max and current, for stats that are not meant to not use the max value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value"></param>
    public void SetStat(TStatEnum stat, float value)
    {
        SetMax(stat, value);
        SetValue(stat: stat, value);
        Debug.Assert(GetValue(stat: stat) == value, "The value does not match.");
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

    public void BoostAdd(IStatBooster booster)
    {
        if (booster.GetBoostSource() == INVALID_BOOST_SOURCE){throw new Exception("Invalid source.");}
        var boost = booster.GetStatBoost();
        if (boost is not null)
        {
            Boosts[booster.GetBoostSource()] = boost;
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
    public Dictionary<TStatEnum, ClampFloat> GetStatDictionary()
    {
        return Contents;
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
        public Dictionary<TStatEnum, float> MaxAdditiveBonus = new();
        public Dictionary<TStatEnum, float> MaxMultiplicativeBonus = new();

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
            MaxMultiplicativeBonus.ContainsKey(stat) ? MaxMultiplicativeBonus[stat] : 0;
        
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

