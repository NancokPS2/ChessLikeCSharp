using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
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

	public override void Setup(Mob mob)
	{
		base.Setup(mob);
		AbilityActivated.Owner = TargetMob;
	}

	public override string GetDescription(bool includeBase = true)
	{
		string output = base.GetDescription(includeBase)
			.Format(
				new()
				{
					{"Ability", AbilityActivated.Name},
				}
			);
		output = output.AddBreak(1);
		output = output.NewLine($"[{AbilityActivated.Name}]");
		output = output.NewLine(AbilityActivated.GetDescription(true, false));

		return output;
	}

	protected override void Use()
	{
		base.Use();
		AbilityActivated.Owner = TargetMob;
		UsageParameters parameters = new(TargetMob, CombatScene.GetGrid(), AbilityActivated);
		parameters.PositionsTargeted.Add(TargetMob.GetPosition());
		parameters.UpdateAffectedCells();
		parameters.UpdateMobsTargeted();


		//AbilityActivated.Use(parameters);
		EventBus.ActionQueueRequested?.Invoke(parameters);
	}
}
