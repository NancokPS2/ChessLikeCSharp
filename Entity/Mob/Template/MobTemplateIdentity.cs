using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplateIdentity : MobTemplate
{
	[Export]
	public Godot.Collections.Array<string> Names = new();

	[Export]
	public Godot.Collections.Array<EFaction> Factions = new();

	public MobTemplateIdentity() : base(ETemplateType.IDENTITY)
	{
	}

    public override Mob ApplyTemplate(Mob mob)
    {
        //Abilities
        ApplyNames(mob, Names);

        //StatBoosts
        ApplyFactions(mob, Factions);

        return mob;
    }
}
