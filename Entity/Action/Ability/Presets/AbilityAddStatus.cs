using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.StatusEffect;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityAddStatus : Ability
{
	[Export]
	protected Status StatusEffectToApply = null!;

	public override void Setup(Mob mob)
	{
		base.Setup(mob);
	}

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);
		foreach (var item in usageParams.MobsTargeted)
		{
			item.AddStatusEffect((Status)StatusEffectToApply.Duplicate(true));
		}
	}

	public override string GetDescription(bool includeBasics = true, bool assumIsSetup = true)
	{
		string output = base.GetDescription(includeBasics)
		.Format(
			new()
			{
				{"Status", StatusEffectToApply.Name}
			}
		);
		output = output.AddBreak(2);
		output = output.NewLine($"[Status: {StatusEffectToApply.Name}]");
		output = output.NewLine(StatusEffectToApply.GetDescription());

		return output;
	}
}
