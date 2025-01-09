using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Command;
using ChessLike.Extension;

namespace ChessLike.Entity.Action.Preset;

public class AbilityPunch : Ability
{
    public AbilityPunch() : base()
    {

        ChainName("Attack");
        ChainIdentifier(EAbility.PUNCH);
        ChainFlag(EActionFlag.DEALS_DAMAGE);
        ChainFlag(EActionFlag.HOSTILE);

        TargetParams = new TargetingParameters(){
            TargetingRange = 1,
            TargetingNeedsValidMob = true,
            TargetingMaxPositions = 1,
        };

        FilterParams = new MobFilterParameters()
        {
            
        };
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
        => usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) / 2;

    public override string GetUseText(UsageParameters parameters)
    {
        string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
        return $"{Owner.DisplayedName} attacked {targets} for {GetDamage(parameters)} damage";
    }

    public override string GetDescription()
    {
        return $"Attack a target with your weapon, dealing {Owner.Stats.GetValue(StatName.STRENGTH) / 2} damage. (50% {StatName.STRENGTH})";
    }
}
