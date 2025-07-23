using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Extension;
using ChessLike.Storage;
using Godot;
using Godot.Collections;

namespace ChessLike.Entity;

public partial class MobTemplate : Resource
{
    public enum ETemplateType
    {
        INVALID = -1,
        JOB,
        RACE,
        BASE,
        EXTRA }

    public readonly ETemplateType Type;



    [Export]
    public string TemplateName = "Unnamed Template";

    public MobTemplate(ETemplateType type)
    {
        Type = type;
    }

    public MobTemplate() : this(ETemplateType.INVALID)
    {
    }

    public virtual Mob ApplyTemplate(Mob mob){ throw new NotImplementedException(); }

    protected void ApplyAbilities(Mob mob, Godot.Collections.Array<Ability> abilities)
    {
        foreach (var item in abilities)
        {
            mob.AddAction(item);
        }
    }

    protected void ApplyStatBoosts(Mob mob, Godot.Collections.Array<MobStatBoost> statBoosts)
    {
        string boostSource = Type.ToString();
        MobStatBoost finalBoost = new(boostSource);
        foreach (var item in statBoosts)
        {
            item.Source = boostSource;
            finalBoost += item;
        }
        mob.Stats.BoostAdd(finalBoost, true);
    }

    protected void ApplyBaseStats(Mob mob, MobStatSet statSet)
    {
        GD.Print($"Replacing stats of {mob.DisplayedName} with stats from template {ResourcePath}");
        mob.Stats = statSet ?? mob.Stats;
    }
    public override string ToString()
    {
        return $"{Type} - {TemplateName}";
    }

}
