using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Command;

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
        ChainFlag(EFlag.DEALS_DAMAGE);
        ChainFlag(EFlag.HOSTILE);

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
                ChainFlag(EFlag.UNARMED);
                break;
        }
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        float damage = usage_params.OwnerRef.Stats.GetValue(StatName.STRENGTH);
        MobCommandTakeDamage command = new MobCommandTakeDamage(damage);
        usage_params.MobsTargeted.ForEach( x => x.CommandProcess(command));
    }


    public float GetDamage(UsageParameters usage) => VariantCurrent switch
    {
        AbilityVariant.WEAPON => 
            usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) / 2,
        AbilityVariant.UNARMED =>
            usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) / 2,
        _ => 0,
    };
}
