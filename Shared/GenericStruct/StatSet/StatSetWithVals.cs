using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.GenericStruct.StatSet;

public partial class StatSetWithValues<[MustBeVariant] TStatEnum, TValueEnum> : StatSet<TStatEnum>
where TStatEnum : notnull, Enum
where TValueEnum : notnull, Enum
{
    public readonly TValueEnum INVALID_VALUE_ENUM;
    public readonly TValueEnum[] AllValues;
    public readonly BiDictionary<TValueEnum, TStatEnum> ValueToStatDict;

    public StatSetWithValues() : base()
    {
        AllValues = (TValueEnum[])(
            from TValueEnum e
            in Enum.GetValues(typeof(TValueEnum))
            where IsValidValue(e)
            select e
            );

        foreach (TValueEnum stat in AllValues)
        {
            ValueDict[stat] = new();
        }
    }

    public Dictionary<TValueEnum, float> ValueDict { get; set; } = new();

    protected void SetAssociatedStat(TValueEnum valKey, TStatEnum stat)
        => ValueToStatDict[valKey] = stat;

    public TStatEnum GetAssociatedStat(TValueEnum val)
        => ValueToStatDict.ContainsKey(val) ? ValueToStatDict.Get(val) : INVALID_STAT_ENUM;

    public TValueEnum GetAssociatedValue(TStatEnum stat)
        => ValueToStatDict.ContainsValue(stat) ? ValueToStatDict.GetReversed(stat) : INVALID_VALUE_ENUM;

    #region Modify stats

    public void Refill()
    {
        foreach (var item in AllValues)
        {
            SetValue(item, GetMax(item));
        }
    }

    public float ChangeValue(TValueEnum stat, float amount)
    {
        float original_value = GetValue(stat);
        SetValue(stat, GetValue(stat) + amount);
        float final_value = GetValue(stat);
        return final_value - original_value;
    }


    public void SetValue(TValueEnum valKey, float value)
    {
        var associatedStat = GetAssociatedStat(valKey);
        float original_val;
        ValueDict.TryGetValue(valKey, out original_val);
        float max = GetStat(associatedStat);
        ValueDict[valKey] = MathF.Min(value, max);
    }

    public float GetValue(TValueEnum valKey)
    {
        var val = ValueDict[valKey];
        return Mathf.Snapped(
            MathF.Min(
                val,
                GetMax(valKey)),
            0.1f
            );
    }

    public float GetValue(TStatEnum stat)
    {
        return GetValue(GetAssociatedValue(stat));
    }

    public float GetValuePrecent(TValueEnum valKey)
    {
        return GetValue(valKey) / GetMax(valKey);
    }


    public void SetValuePercent(TValueEnum stat, float percent)
    {
        if (percent > 1.0f || percent < 0.0f)
        {
            throw new ArgumentOutOfRangeException("Must be a float from 0 to 1.");
        }
        SetValue(stat, GetMax(stat) * percent);
    }


    public float GetMax(TValueEnum valKey)
    {
        var associatedStat = GetAssociatedStat(valKey);
        return associatedStat is not null ? GetStat(associatedStat) : float.MaxValue;
    }

    public virtual bool IsValidValue(TValueEnum val)
        => true;
    #endregion
}
