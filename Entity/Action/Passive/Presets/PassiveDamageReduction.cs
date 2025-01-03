using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Command;
using ChessLike.World;

namespace ChessLike.Entity.Action;

public class PassiveDamageReduction : Passive
{
    public float Percentage = 0.15f;
    private IncomingDamageModifier? _command_interceptor_ref;

    public PassiveDamageReduction(int TurnDuration) : base()
    {
        DurationParams = new(TurnDuration, null, null);
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
    }

    public override void OnAddedToMob()
    {
        base.OnAddedToMob();
        _command_interceptor_ref = new IncomingDamageModifier(Percentage);
        Owner.CommandAddInterceptor(_command_interceptor_ref);
    }
    public override void OnRemovedFromMob()
    {
        base.OnRemovedFromMob();
        Owner.CommandRemoveInterceptor(_command_interceptor_ref ?? throw new Exception("The command was removed ahead of time!?"));
        _command_interceptor_ref = null;
    }
    public override string GetDescription()
    {
        return $"Reduce damage taken by {Percentage*100}%.";
    }
}
