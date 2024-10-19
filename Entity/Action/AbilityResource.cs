using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityResource : Godot.Resource
{

    [Export]
    public EAbility Identifier;
    
    [Export]
    public string Name = "";

    [Export]
    public int PriorityDefault;

    [ExportCategory("Filter Parameters")]
    [Export]
    public bool OnlyAffectOwner = false;
    [Export]
    public bool CanAffectMob = true; 
    [Export]
    //The target cannot be an ally.
    public bool CannotAffectAlly = false; 
    [Export]
    //The target cannot be an enemy.
    public bool CannotAffectEnemy = false;
    [Export]
    //The target must be below this health.
    public float MaximumHealthPercent = 1.0f;

    [ExportCategory("Targeting Parameters")]
    [Export]
    public uint TargetingRange = 4;
    [Export]
    public StatName TargetingRangeStatBonus = (StatName)(-1);
    [Export]
    public int MaxTargetedPositions = 1;
    [Export]
    public bool RespectsOwnerPathing = false;

    [Export]
    public Ability.TargetingParameters.AoEMode AoeShape = Ability.TargetingParameters.AoEMode.SINGLE;
    [Export]
    //Area when in SINGLE mode.
    public uint AoERange = 0;

    [ExportCategory("Animation Parameters")]
    [Export]
    public float Duration = 1f;

    //[ExportCategory("EffectList")]
    //TODO
    [ExportCategory("Passive Parameters")]
    [Export]
    public Godot.Collections.Array<Ability.EPassiveTrigger> Triggers = new();

}
