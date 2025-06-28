using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public partial class MobCommandWalk : Command
{
    List<Vector3i> Path;

    public MobCommandWalk()
    {
    }

    public MobCommandWalk(List<Vector3i> path)
    {
        Path = path;
    }

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
        mob.MoveTroughPath(Path);
    }
}
