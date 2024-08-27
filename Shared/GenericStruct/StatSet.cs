using System.Numerics;
using System.Xml;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Shared.GenericStruct;
public class StatSet<StatEnum> where StatEnum : Enum
{
    Dictionary<StatEnum, ClampFloat> Contents {get; set;} = new();


    public StatSet()
    {
        foreach (StatEnum stat in Enum.GetValues(typeof(StatEnum)))
        {
            Contents[stat] = new();
        }
    }

    public StatSet(Dictionary<StatEnum, float> values) : base()
    {
        foreach (StatEnum stat_name in values.Keys)
        {
            SetStat(stat_name, values[stat_name]);
        }
    }


    public void ChangeValue(StatEnum stat, float amount, ClampFloat.Modifier[]? modifiers = null)
    {
        Contents[stat].ChangeValue(amount, modifiers);
    }

    public void SetValue(StatEnum stat, float value)
    {
        Contents[stat].SetCurrent(value);
    }

    public void SetValuePercent(StatEnum stat, float percent)
    {
        if (percent > 1.0f || percent < 0.0f)
        {
            throw new ArgumentOutOfRangeException("Must be a float from 0 to 1.");
        }
        SetValue(stat, GetMax(stat) * percent);
    }

    public void SetMax(StatEnum stat, float value)
    {
        Contents[stat].SetMax(value);
    }

    public void SetMin(StatEnum stat, float value)
    {
        if(value < 0){throw new ArgumentOutOfRangeException("Negative minimums are not supported yet.");}
        Contents[stat].SetMin(value);
    }

    public float GetValue(StatEnum stat)
    {
        ClampFloat output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetCurrent();
    }

    public float GetValuePrecent(StatEnum stat)
    {
        return GetValue(stat) / GetMax(stat);
    }

    public float GetMax(StatEnum stat)
    {
        ClampFloat output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetMax();
    }

    public float GetMin(StatEnum stat)
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
    public void SetStat(StatEnum stat, float value)
    {
        SetMax(stat, value);
        SetValue(stat: stat, value);
    }

    public void MultiplyStat(StatEnum stat, float multiplier)
    {
        float current = GetValue(stat);
        SetStat(stat, current * multiplier);
    }

    public void MultiplyStat(StatEnum stat, double multiplier)
    {
        MultiplyStat(stat, (float)multiplier);
    }

    public void SetTypes(StatEnum stat_name, string[] types)
    {
        Contents[stat_name].ClearTypes();
        foreach (string type in types)
        {
            Contents[stat_name].AddType(type);
        }
    }

    //Resets the specified stats to their maximum value. If none are specified, all of them are reset.
    public void SetToMax(StatEnum[]? stats = null)
    {
        //Set ALL stats if null.
        stats ??= GetArrayOfNames();

        foreach (StatEnum stat in stats)
        {
            SetStat(stat, GetMax(stat));
        }
    }

    public static StatEnum[] GetArrayOfNames()
    {
        int total_stats = Enum.GetNames(typeof(StatEnum)).Length;
        StatEnum[] output = new StatEnum[total_stats];

        int index = 0;
        foreach (StatEnum name in Enum.GetValues(typeof(StatEnum)))
        {
            output[index] = name;
            index ++;
        }
        return output;
    }

    public static StatSet<StatEnum> GetAverage(StatSet<StatEnum> a, StatSet<StatEnum> b)
    {
        StatSet<StatEnum> output = new StatSet<StatEnum>();

        foreach (StatEnum stat in GetArrayOfNames())
        {
            float average_max = (a.GetMax(stat) + b.GetMax(stat)) / 2;
            float average_value = (a.GetValue(stat) + b.GetValue(stat)) / 2;

            output.SetMax(stat, average_max);
            output.SetValue(stat, average_value);
        }

        return output;
    }

}

