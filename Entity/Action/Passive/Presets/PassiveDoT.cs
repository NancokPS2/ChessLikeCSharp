using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Command;
using ChessLike.World;

namespace ChessLike.Entity.Action;

public class PassiveDoT : Passive
{
    public float Percentage = 0.15f;

    public PassiveDoT(int TurnDuration) : base()
    {
        IsTriggeredByTurnEnd = true;
        DurationParams = new(TurnDuration, null, null);
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        float health_to_loose = usage_params.OwnerRef.Stats.GetMax(StatName.HEALTH) * Percentage;
        MobCommand command = new MobCommandTakeDamage(health_to_loose){DefenseRatioAccounted = 100};
        usage_params.OwnerRef.CommandProcess(command);
    }

    public override string GetDescription()
    {
        return $"Take {Percentage*100}% of damage at the end of its turn." + base.GetDescription();
    }
}
