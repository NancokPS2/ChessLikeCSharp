using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplateJob : MobTemplate
{
    [Export]
    protected Godot.Collections.Array<Ability> Abilities = new();

    [Export]
    protected Godot.Collections.Array<MobStatBoost> StatBoosts = new();

    public MobTemplateJob() : base(ETemplateType.JOB)
    {
    }

    public override Mob ApplyTemplate(Mob mob)
    {
        //Abilities
        ApplyAbilities(mob, Abilities);

        //StatBoosts
        ApplyStatBoosts(mob, StatBoosts);

        return mob;
    }

}
