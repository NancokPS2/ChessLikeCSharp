using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Command;
using ChessLike.Extension;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity.Action.Preset;

public class AbilityWeaponAttack : Ability
{
    public enum AbilityVariant {LIGHT_BLADE, SPEAR, HEAVY_BLUNT, BOW}
    public AbilityVariant Variant;
    public float WeaponDamage;
    public AbilityWeaponAttack(AbilityVariant variant, float weapon_damage) : base()
    {
        Variant = variant;
        WeaponDamage = weapon_damage;

        ChainName("Weapon Attack");
        ChainIdentifier(EAbility.WEAPON_ATTACK);
        ChainFlag(EActionFlag.DEALS_DAMAGE);
        ChainFlag(EActionFlag.HOSTILE);

        TargetParams = new TargetingParameters(){
            TargetingRange = GetRange(),
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

    public float GetDamage(UsageParameters usage) => GetDamage(usage, Variant);

    public float GetDamage(UsageParameters usage, AbilityVariant variant) => variant switch
    {
        AbilityVariant.HEAVY_BLUNT =>
            (usage.OwnerRef.Stats.GetValue(StatName.STRENGTH) * 0.9f) 
            + WeaponDamage,

        AbilityVariant.LIGHT_BLADE => 
            (usage.OwnerRef.Stats.GetValue(StatName.AGILITY) * 0.6f) 
            + (usage.OwnerRef.Stats.GetValue(StatName.AGILITY) * 0.2f)
            + WeaponDamage,

        AbilityVariant.BOW =>
            (usage.OwnerRef.Stats.GetValue(StatName.AGILITY) * 0.2f)
            + (usage.OwnerRef.Stats.GetValue(StatName.INTELLIGENCE) * 0.15f) 
            + WeaponDamage,

        AbilityVariant.SPEAR => GetDamage(usage, Variant),

        _ => WeaponDamage,
    };

    public uint GetRange() => Variant switch
    {
        AbilityVariant.SPEAR => 2,
        AbilityVariant.BOW => 6,
        _ => 1,
    };

    public override string GetUseText(UsageParameters parameters)
    {
        string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
        return $"{Owner.DisplayedName} attacked {targets} for {GetDamage(parameters)} damage.";
    }

    public override string GetDescription() 
    {
        string output;
        output = Variant switch
        {
            AbilityVariant.LIGHT_BLADE => $"Slash the target.",
            _ => "Attack with your weapon.",
        };

        return output;
    }
}
