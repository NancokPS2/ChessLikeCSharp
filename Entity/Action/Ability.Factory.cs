using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Ability
{

    public static Ability Create(EAbility identity_enum)
    {
        Ability output = new();
        output = identity_enum switch
        {
            EAbility.PUNCH => output
                .ChainName("Punch")
                .ChainEffectDamageHealth(StatName.STRENGTH, 0.5f),
            EAbility.HEAL => output
                .ChainName("Heal")
                .ChainHealHealth(25),
            EAbility.MOVE => output
                .ChainName("Move")
                .ChainEffectMove(),
            _ => throw new Exception("Non existent enum value."),
        };

        output.ChainIdentifier(identity_enum);
        return output;
        
    }
    public Ability ChainIdentifier(EAbility identifier)
    {
        Identifier = identifier;
        return this;
    }

    public Ability ChainName(string name)
    {
        Name = name;
        return this;
    }
    

    public Ability ChainEffectDamageHealth(StatName stat_based, float damage)
    {
        //Effect
        EffectStatChange effect = new();
        effect.SetOwnerAddingBoost(StatName.STRENGTH, 1);
        EffectParams.Add(effect);

        //Targeting
        TargetParams.TargetingRange = 1;

        return this;
    }

    public Ability ChainHealHealth(float amount)
    {
        //Effect
        EffectStatChange effect = new();
        effect.SetFlatAmount(amount);
        EffectParams.Add(effect);

        //Targeting
        TargetParams.TargetingRange = 1;

        return this;
    }

    public Ability ChainEffectMove()
    {
        WalkEffect effect = new();
        EffectParams.Add(effect);

        //TargetParams
        TargetParams.TargetingRangeStatBonus = StatName.MOVEMENT;
        TargetParams.TargetingRange = 0;
        TargetParams.NeededVacancy = TargetingParameters.VacancyStatus.HAS_NO_MOB;
        TargetParams.RespectsOwnerPathing = true;

        FilterParams.CanAffectMob = false;

        return this;
    }

    public Ability ChainTargetBoostRangeByStat( StatName stat)
    {
        TargetParams.TargetingRangeStatBonus = stat;
        return this;
    }


}