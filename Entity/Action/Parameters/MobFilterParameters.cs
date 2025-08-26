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
	
    /// <summary>
	/// The target cannot be the owner.
	/// </summary>
	[Export]
    public bool CannotAffectOwner = false;

    /// <summary>
	/// The target cannot be an ally.
	/// </summary>
    [Export]
    public bool CannotAffectAlly = false;

    /// <summary>
	/// The target cannot be an enemy.
	/// </summary>
    [Export]
    public bool CannotAffectEnemy = false;

    /// <summary>
	/// The target must be below this health percentage.
	/// </summary>
    [Export]
    public float MaximumHealthPercent = 1.0f;

    public override string ToString()
    {
        return this.GetFieldValuesAsDict().ToStringList();
    }
}
