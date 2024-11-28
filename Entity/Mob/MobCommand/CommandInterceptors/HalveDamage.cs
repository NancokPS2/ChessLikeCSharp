using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Command;

public class HalveDamage : MobCommandInterceptor
{
    public override bool CanIntercept(MobCommand command)
    {
        return command is MobCommandTakeDamage;
    }

    public override void UseInterceptor(MobCommand command)
    {
        float reduction = (command as MobCommandTakeDamage).Damage /= 2;
        (command as MobCommandTakeDamage).Damage -= reduction;

        MobCommand.Broadcaster.Broadcast(new(){
            {EInfo.DAMAGE_REDUCED, reduction.ToString()}
        });
    }
    public void Use(MobCommandTakeDamage command)
    {

    }
}
