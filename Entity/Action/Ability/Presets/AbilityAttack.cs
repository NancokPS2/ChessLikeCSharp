using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.Action.Preset;

[GlobalClass]
public partial class AbilityAttack : Ability
{
    public AbilityAttack() : base()
    {
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        float damage = GetDamage(usage_params);
        MobCommandTakeDamage command = new MobCommandTakeDamage(damage);
        foreach (var item in usage_params.MobsTargeted)
        {
            item.CommandProcess(command);
        }
    }


    public float GetDamage(UsageParameters usage)
        => usage.OwnerRef.Stats.GetStat(EStatName.STRENGTH) / 2;

    public override string GetUseText(UsageParameters parameters)
    {
        string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
        return $"{Owner.DisplayedName} attacked {targets} for {GetDamage(parameters)} damage";
    }

    public override string GetDescription()
    {
        return $"Attack a target with your weapon, dealing {Owner.Stats.GetStat(EStatName.STRENGTH) / 2} damage. (50% {EStatName.STRENGTH})";
    }
}
