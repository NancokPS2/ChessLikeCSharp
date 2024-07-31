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
        SPEED,
        DELAY,
    }
    Dictionary<Name, FloatRes> Contents {get; set;} = new();


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


    public void ChangeValue(Name stat, float amount, FloatRes.Modifier[]? modifiers = null)
    {
        Contents[stat].ChangeCurrent(amount, modifiers);
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
        FloatRes output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetCurrent();
    }

    public float GetValuePrecent(Name stat)
    {
        return GetValue(stat) / GetMax(stat);
    }

    public float GetMax(Name stat)
    {
        FloatRes output = new();
        Contents.TryGetValue(stat, out output);
        return output.GetMax();
    }

    public float GetMin(Name stat)
    {
        FloatRes output = new();
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

    public void SetTypes(Name stat_name, string[]? types)
    {
        Contents[stat_name].ClearTypes();
        foreach (string type in types)
        {
            Contents[stat_name].AddType(type);
        }
    }

}
