using System;
using System.Diagnostics;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobStatBoost : StatBoost<EStatName>
{
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
