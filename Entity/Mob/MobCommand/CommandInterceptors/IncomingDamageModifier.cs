using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Command;

public class IncomingDamageModifier : MobCommandInterceptor
{
    public float Percentage;
    public bool OnlyDirectHarm = true;

    public IncomingDamageModifier(float percentage)
    {
        Percentage = percentage;
    }
    public override bool CanIntercept(MobCommand command)
    {
        bool is_damage = command is MobCommandTakeDamage;
        bool respects_direct = OnlyDirectHarm && command.Flags.Contains(ECommandFlag.DIRECT)
        || !OnlyDirectHarm;
        return is_damage && respects_direct;
    }

    public override void UseInterceptor(MobCommand command)
    {
        if (command is not MobCommandTakeDamage){throw new Exception("Invalid command, CanIntercept() should have failed already.");}

        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        float reduction = (command as MobCommandTakeDamage).Damage *= Percentage;
        (command as MobCommandTakeDamage).Damage -= reduction;
        #pragma warning restore CS8602 // Dereference of a possibly null reference.

        MobCommand.Broadcaster.Broadcast(new(){
            {EInfo.DAMAGE_REDUCED, reduction.ToString()}
        });
    }
    public void Use(MobCommandTakeDamage command)
    {

    }
}
