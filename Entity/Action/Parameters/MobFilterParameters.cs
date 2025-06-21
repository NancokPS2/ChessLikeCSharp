using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.Action;
//Defines what can be affected with this and how.
[GlobalClass]
public partial class MobFilterParameters : Resource
{
    //If a mob is standing in a targeted location, they are included in the mob list of the UsageParameters.
    [Export]
    public bool PickMobInTargetPos = true;
    //Only the owner is a valid target. Should have a range of 0.
    [Export]
    public bool OnlyAffectOwner = false;

    //The target cannot be an ally.
    [Export]
    public bool CannotAffectAlly = false;

    //The target cannot be an enemy.
    [Export]
    public bool CannotAffectEnemy = false;

    //The target must be below this health.
    [Export]
    public float MaximumHealthPercent = 1.0f;

    public override string ToString()
    {
        return this.GetFieldValuesAsDict().ToStringList();
    }
}
