using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobStatBoost : StatBoost<EStatName>
{
    [Export]
    private Godot.Collections.Dictionary<EStatName, float> maxAdditiveBonus
    {
        set => MaxAdditiveBonus = new(value);
        get => new(MaxAdditiveBonus);
    }

    [Export]
    private Godot.Collections.Dictionary<EStatName, float> maxMultiplicativeBonus
    {
        set => MaxMultiplicativeBonus = new(value);
        get => new(MaxMultiplicativeBonus);
    }

    public MobStatBoost() : base("UNDEFINED")
    {
    }

    public MobStatBoost(string Source) : base(Source)
    {

    }

    public MobStatBoost(StatBoost<EStatName> statSet) : base(statSet)
    {
    }

    public static MobStatBoost operator +(MobStatBoost source, MobStatBoost added)
    {
        StatBoost<EStatName> output = Combined(source, added);
        return new(output);
    }
}
