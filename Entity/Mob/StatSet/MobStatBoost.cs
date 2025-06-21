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
}
