using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static ChessLike.Entity.Action;

namespace ChessLike.Entity;

public partial class Action
{

    public static Action Create(EAction identity_enum)
    {
        Action output = new();
        output = identity_enum switch
        {
            EAction.PUNCH => output
                .ChainName("Punch")
                .ChainEffectDamageHealth(StatName.STRENGTH, 0.5f),
            EAction.HEAL => output
                .ChainName("Heal")
                .ChainEffectHealPrecentage(0.25f),
            EAction.MOVE => output
                .ChainName("Move")
                .ChainEffectMove(),
            _ => throw new Exception("Non existent enum value."),
        };

        output.ChainIdentifier(identity_enum);
        return output;
        
    }
    public Action ChainIdentifier(EAction identifier)
    {
        Identifier = identifier;
        return this;
    }

    public Action ChainName(string name)
    {
        Name = name;
        return this;
    }
    

    public Action ChainEffectDamageHealth(StatName stat_based, float damage)
    {
        //Effect
        AttackEffect effect = new();
        EffectParams.Add(effect);

        //Targeting
        TargetParams.TargetingRange = 1;

        return this;
    }

    public Action ChainEffectHealPrecentage(float percent)
    {
        RecoverPercentOfMaxEffect effect = new();
        effect.percentage = percent;
        EffectParams.Add(effect);

        return this;
    }

    public Action ChainEffectMove()
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

    public Action ChainTargetBoostRangeByStat( StatName stat)
    {
        TargetParams.TargetingRangeStatBonus = stat;
        return this;
    }


}