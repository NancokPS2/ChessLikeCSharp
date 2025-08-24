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
    public readonly BiDictionary<TValueEnum, TStatEnum> ValueToStatDict = new();

    public StatSetWithValues(TStatEnum invalidStat, TValueEnum invalidValue) : base(invalidStat)
    {
        INVALID_VALUE_ENUM = invalidValue;

        AllValues = (
            from TValueEnum e
            in Enum.GetValues(typeof(TValueEnum))
            where IsValidValue(e)
            select e
            ).ToArray();

        foreach (TValueEnum stat in AllValues)
        {
            ValueDict[stat] = new();
        }
    }

    public Dictionary<TValueEnum, float> ValueDict { get; set; } = new();

    #region Associated Stats
    protected void SetAssociatedStat(TValueEnum valKey, TStatEnum stat)
        => ValueToStatDict[valKey] = stat;

    public TStatEnum GetAssociatedStat(TValueEnum val)
        => HasStatAssociatedToValue(val) ? ValueToStatDict[val] : INVALID_STAT_ENUM;

    public TValueEnum GetAssociatedValue(TStatEnum stat)
        => HasValueAssociatedToStat(stat) ? ValueToStatDict[stat] : INVALID_VALUE_ENUM;

    public bool HasStatAssociatedToValue(TValueEnum whichValue)
        => ValueToStatDict.ContainsKey(whichValue);

    public bool HasValueAssociatedToStat(TStatEnum whichStat)
        => ValueToStatDict.ContainsValue(whichStat);

    #endregion

	#region Setters

    public void RefillValues(TValueEnum[]? values = null)
    {
        foreach (var item in values ?? AllValues)
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

    public void SetValuePercent(TValueEnum stat, float percent)
    {
        if (percent > 1.0f || percent < 0.0f)
        {
            throw new ArgumentOutOfRangeException("Must be a float from 0 to 1.");
        }
        SetValue(stat, GetMax(stat) * percent);
    }

    #region Getters
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

    public float GetValueByStat(TStatEnum stat)
    {
        if (!HasValueAssociatedToStat(stat)) throw new Exception();
        return GetValue(GetAssociatedValue(stat));
    }

    public float GetValuePrecent(TValueEnum valKey)
    {
        return GetValue(valKey) / GetMax(valKey);
    }

    public float GetMax(TValueEnum valKey)
    {
        TStatEnum associatedStat = GetAssociatedStat(valKey);
		if (Convert.ToInt64(associatedStat) != Convert.ToInt64(INVALID_STAT_ENUM))
		{
			return GetStat(associatedStat);
		}
		else
		{
			return float.MaxValue;
		}
    }

    public Dictionary<TValueEnum, float> GetValueDictionary()
        => new(ValueDict);
    #endregion

    public virtual bool IsValidValue(TValueEnum val)
        => true;
    #endregion
}
