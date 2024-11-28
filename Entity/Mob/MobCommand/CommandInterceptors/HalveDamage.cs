using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Command;

public class HalveDamage : MobCommandInterceptor
{
    public bool OnlyDirectHarm = true;
    public override bool CanIntercept(MobCommand command)
    {
        bool is_damage = command is MobCommandTakeDamage;
        bool respects_direct = OnlyDirectHarm && command.Flags.Contains(ECommandFlag.DIRECT)
        || !OnlyDirectHarm;
        return is_damage && respects_direct;
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
