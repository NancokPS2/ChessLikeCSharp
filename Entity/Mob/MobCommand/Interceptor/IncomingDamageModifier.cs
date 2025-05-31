using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.MobCommand;

public class IncomingDamageModifier : Interceptor
{
    public float Percentage;
    public bool OnlyDirectHarm = true;

    public IncomingDamageModifier(float percentage)
    {
        Percentage = percentage;
    }
    public override bool CanIntercept(Command command)
    {
        bool is_damage = command is MobCommandTakeDamage;
        bool respects_direct = OnlyDirectHarm && command.Flags.Contains(ECommandFlag.DIRECT)
        || !OnlyDirectHarm;
        return is_damage && respects_direct;
    }

    public override void UseInterceptor(Command command)
    {
        if (command is not MobCommandTakeDamage){throw new Exception("Invalid command, CanIntercept() should have failed already.");}

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        float reduction = (command as MobCommandTakeDamage).Damage *= Percentage;
        (command as MobCommandTakeDamage).Damage -= reduction;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        EventBus.MobCommandBroadcasted?.Invoke(new(){
            {EInfo.DAMAGE_REDUCED, reduction.ToString()}
        });
    }
}
