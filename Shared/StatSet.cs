using System.Numerics;
using System.Xml;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Shared;
public class StatSet
{
    public enum Name
    {
        HEALTH,
        ENERGY,
        MOVEMENT,
        DELAY,
        STRENGTH,
        AGILITY,
        INTELLIGENCE,
    }
    Dictionary<Name, ClampFloat> Contents {get; set;} = new();


    public StatSet()
    {
        foreach (StatSet.Name stat in Enum.GetValues(typeof(StatSet.Name)))
        {
            Contents[stat] = new();
        }
    }

    public StatSet(Dictionary<Name, float> values) : base()
    {
        foreach (Name stat_name in values.Keys)
        {
            SetStat(stat_name, values[stat_name]);
        }
    }


    public void ChangeValue(Name stat, float amount, ClampFloat.Modifier[]? modifiers = null)
    {
        Contents[stat].ChangeValue(amount, modifiers);
    }

    public void SetValue(Name stat, float value)
    {
        Contents[stat].SetCurrent(value);
    }

    public void SetValuePercent(Name stat, float percent)
    {
        if (percent > 1.0f || percent < 0.0f)
        {
            throw new ArgumentOutOfRangeException("Must be a float from 0 to 1.");
        }
        SetValue(stat, GetMax(stat) * percent);
    }

    public void SetMax(Name stat, float value)
    {
        Contents[stat].SetMax(value);
    }

    public void SetMin(Name stat, float value)
    {
        if(value < 0){throw new ArgumentOutOfRangeException("Negative minimums are not supported yet.");}
        Contents[stat].SetMin(value);
    }

    public float GetValue(Name stat)
    {
        ClampFloat output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetCurrent();
    }

    public float GetValuePrecent(Name stat)
    {
        return GetValue(stat) / GetMax(stat);
    }

    public float GetMax(Name stat)
    {
        ClampFloat output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetMax();
    }

    public float GetMin(Name stat)
    {
        ClampFloat output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetMin();
    }


    /// <summary>
    /// Set max and current, for stats that are not meant to not use the max value.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="value"></param>
    public void SetStat(Name stat, float value)
    {
        SetMax(stat, value);
        SetValue(stat: stat, value);
    }

    public void MultiplyStat(Name stat, float multiplier)
    {
        float current = GetValue(stat);
        SetStat(stat, current * multiplier);
    }

    public void MultiplyStat(Name stat, double multiplier)
    {
        MultiplyStat(stat, (float)multiplier);
    }

    public void SetTypes(Name stat_name, string[] types)
    {
        Contents[stat_name].ClearTypes();
        foreach (string type in types)
        {
            Contents[stat_name].AddType(type);
        }
    }

    //Resets the specified stats to their maximum value. If none are specified, all of them are reset.
    public void SetToMax(Name[]? stats = null)
    {
        //Set ALL stats if null.
        stats ??= GetArrayOfNames();

        foreach (Name stat in stats)
        {
            SetStat(stat, GetMax(stat));
        }
    }

    public static Name[] GetArrayOfNames()
    {
        int total_stats = Enum.GetNames(typeof(Name)).Length;
        Name[] output = new Name[total_stats];

        int index = 0;
        foreach (Name name in Enum.GetValues(typeof(Name)))
        {
            output[index] = name;
            index ++;
        }
        return output;
    }

    public static StatSet GetAverage(StatSet a, StatSet b)
    {
        StatSet output = new();

        foreach (Name stat in GetArrayOfNames())
        {
            float average_max = (a.GetMax(stat) + b.GetMax(stat)) / 2;
            float average_value = (a.GetValue(stat) + b.GetValue(stat)) / 2;

            output.SetMax(stat, average_max);
            output.SetValue(stat, average_value);
        }

        return output;
    }

}

