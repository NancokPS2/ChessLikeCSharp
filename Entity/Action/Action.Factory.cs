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
                .ChainEffectDamageHealth(StatName.STRENGTH, 0.5f),
            EAction.HEAL => output
                .ChainEffectHealPrecentage(0.25f),
            _ => throw new Exception("Non existent enum value."),
        };
        return output;
        
    }
    
    public Action ChainSetCost(float energy_cost, float health_cost)
    {
        throw new NotImplementedException();
        return this;
    }

    public Action ChainEffectDamageHealth(StatName stat_based, float damage)
    {
        //Effect
        Effect.Attack effect = new();
        EffectParams.Add(effect);

        //Targeting
        TargetParams.TargetingRange = 1;

        return this;
    }

    public Action ChainEffectHealPrecentage(float percent)
    {
        Effect.RecoverPercentOfMax effect = new();
        effect.percentage = percent;
        EffectParams.Add(effect);

        return this;
    }

    public Action ChainEffectMove()
    {
        Effect.Walk effect = new();
        EffectParams.Add(effect);

        return this;
    }

    public Action ChainTargetBoostRangeByStat( StatName stat)
    {
        TargetParams.TargetingRangeStatBonus = stat;
        return this;
    }


}