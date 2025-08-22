using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public partial class MobCommandMove : Command
{
	public Vector3i TargetLocation;
	[Export]
    protected Godot.Vector3I targetLocation
	{
		get => TargetLocation.ToGVector3I();
		set => TargetLocation = new(value);
	}

	[Export]
	EMobMovementMode MovementType;

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
        mob.Move(TargetLocation);
    }
}
