using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class PassiveResource : Godot.Resource
{

    [Export]
    public EPassive Identifier;
    
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

    [ExportCategory("Animation Parameters")]
    [Export]
    public float Duration = 1f;

    //[ExportCategory("EffectList")]
    //TODO
    [ExportCategory("Passive Parameters")]
    [Export]
    bool dummy;
    //public Godot.Collections.Array<EPassiveTrigger> Triggers = new();

}
