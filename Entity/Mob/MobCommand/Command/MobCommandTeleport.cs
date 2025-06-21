using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public abstract partial class MobCommandTeleport : Command
{
    Vector3i TargetLocation;

    public MobCommandTeleport(Vector3i target)
    {
        TargetLocation = target;
    }

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
        mob.Move(TargetLocation);
    }
}
