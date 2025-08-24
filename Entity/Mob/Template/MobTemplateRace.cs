using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplateRace : MobTemplate
{

	[Export]
	protected Godot.Collections.Array<Ability> Abilities = new();

	[Export]
	protected Godot.Collections.Array<MobStatBoost> StatBoosts = new();

	[Export]
	protected Godot.Collections.Array<EMovementMode> MovementModes = new();

	public MobTemplateRace() : base(ETemplateType.RACE)
	{
	}

	public override Mob ApplyTemplate(Mob mob)
	{
		ApplyAbilities(mob, Abilities);

		ApplyStatBoosts(mob, StatBoosts);

		return mob;
	}

}
