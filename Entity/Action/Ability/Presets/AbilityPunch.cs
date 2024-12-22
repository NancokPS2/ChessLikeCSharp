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
    public enum AbilityVariant {
        WEAPON,
        UNARMED,
        }

    public AbilityVariant VariantCurrent;
    public AbilityPunch(AbilityVariant variant = AbilityVariant.WEAPON) : base()
    {
        VariantCurrent = variant;

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

        switch(variant)
        {
            case AbilityVariant.UNARMED:
                ChainFlag(EActionFlag.UNARMED);
                break;
        }
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        float damage = usage_params.OwnerRef.Stats.GetValue(StatName.STRENGTH);
        MobCommandTakeDamage command = new MobCommandTakeDamage(damage);
        foreach (var item in usage_params.MobsTargeted)
        {
            item.CommandProcess(command);
        }
    }


    public float GetDamage(UsageParameters usage) => VariantCurrent switch
    {
        AbilityVariant.WEAPON => 
            usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) / 2,
        AbilityVariant.UNARMED =>
            usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) / 2,
        _ => 0,
    };

    public override string GetUseText(UsageParameters parameters)
    {
        string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
        return $"{Owner.DisplayedName} attacked {targets}";

    }
}
