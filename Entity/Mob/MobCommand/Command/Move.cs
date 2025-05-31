using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

public class MobCommandMove : Command
{
    Vector3i TargetLocation;
    public MobCommandMove(Vector3i target)
    {
        TargetLocation = target;
    }

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
        mob.Position = TargetLocation;
    }
}
