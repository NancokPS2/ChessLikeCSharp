using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.StatusEffect;

[GlobalClass]
public partial class StatusActivateAbility : Status
{
	protected Ability AbilityActivated = null!;
	[Export]
	private Ability abilityActivated
	{
		get
		{
			return AbilityActivated;
		}

		set
		{
			AbilityActivated = value;
		}
	}


	protected override void Use()
	{
		base.Use();
		AbilityActivated.Owner = TargetMob;
		UsageParameters parameters = new(TargetMob, CombatScene.GetGrid(), AbilityActivated);
		parameters.PositionsTargeted.Add(TargetMob.GetPosition());
		parameters.UpdateAffectedCells();
		parameters.UpdateMobsTargeted();
		AbilityActivated.Use(parameters);
	}
}
