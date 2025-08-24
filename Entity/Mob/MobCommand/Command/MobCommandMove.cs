using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public partial class MobCommandMove : Command
{
	public List<Vector3i> TargetLocation;
	[Export]
    protected Godot.Collections.Array<Godot.Vector3I> targetLocation
	{
		get => new (from pos in TargetLocation select pos.ToGVector3I());
		set => TargetLocation = new (from pos in value select new Vector3i(pos));
	}

	[Export]
	EMovementMode MovementMode;

	public MobCommandMove(List<Vector3i> path, EMovementMode moveMode)
	{
		MovementMode = moveMode;
		TargetLocation = path;
	}

	public override void UseCommand(Mob mob)
	{
		base.UseCommand(mob);
		mob.Move(
			TargetLocation,
			new(MovementMode)
			);
	}
}
