using System;
using ChessLike.Entity;
using Godot;

namespace ChessLike.Shared;


public partial class StatBoost<[MustBeVariant] TStatEnum> : Resource where TStatEnum : notnull, Enum
{
    public string Source;
    //public Dictionary<TStatEnum, float> ValueAdditiveBonus = new();
    //public Dictionary<TStatEnum, float> ValueMultiplicativeBonus = new();
    [Export]
    protected Godot.Collections.Dictionary<TStatEnum, float> MaxAdditiveBonus = new();

    [Export]
    protected Godot.Collections.Dictionary<TStatEnum, float> MaxMultiplicativeBonus = new();

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
    public Dictionary<TStatEnum, float> GetMaxAdditiveBonusDict()
        => new(MaxAdditiveBonus);

    public Dictionary<TStatEnum, float> GetMaxMultiplicativeBonusDict()
        => new(MaxMultiplicativeBonus);

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

    public static StatBoost<TStatEnum> Combined(StatBoost<TStatEnum> sourcer, StatBoost<TStatEnum> added)
    {
        StatBoost<TStatEnum> output = new(sourcer.Source);
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
            output.SetMultiplicativeMax(item, sourcer_max_mult * added_max_mult);
        }

        return output;
    }

    public static StatBoost<TStatEnum> operator +(StatBoost<TStatEnum> sourcer, StatBoost<TStatEnum> added)
        => Combined(sourcer, added);
}
