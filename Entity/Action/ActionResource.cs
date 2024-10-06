using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class ActionResource : Godot.Resource
{
    [Export]
    public string Name = "";

    [Export]
    public int PriorityDefault;

    [ExportCategory("FilterParameters")]
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

    [ExportCategory("TargetingParameters")]
    [Export]
    public uint TargetingRange = 4;
    [Export]
    public StatName TargetingRangeStatBonus = (StatName)(-1);
    [Export]
    public int MaxTargetedPositions = 1;
    [Export]
    //Only works if there is a mob being hit that was deemed as valid.
    public Action.TargetingParameters.VacancyStatus NeededVacancy = Action.TargetingParameters.VacancyStatus.HAS_MOB;
    [Export]
    public bool RespectsOwnerPathing = false;

    [Export]
    public Action.TargetingParameters.AoEMode AoeShape = Action.TargetingParameters.AoEMode.SINGLE;
    [Export]
    //Area when in SINGLE mode.
    public uint AoERange = 0;

    [ExportCategory("AnimationParameters")]
    [Export]
    public float Duration = 1f;

    //[ExportCategory("EffectList")]
    //TODO

}
