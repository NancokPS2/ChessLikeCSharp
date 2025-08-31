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

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);
		foreach (var item in usageParams.MobsTargeted)
		{
			item.AddStatusEffect((Status)StatusEffectToApply.Duplicate(true));
		}
	}
}
