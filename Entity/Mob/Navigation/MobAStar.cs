using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using Godot;

namespace ChessLike.Entity;

public partial class MobAStar : AStar3D
{
	public override float _ComputeCost(long fromId, long toId)
	{
		var fromPos = GetPointPosition(fromId);
		var toPos = GetPointPosition(toId);
		return Mathf.Abs(fromPos.X - toPos.X) + Mathf.Abs(fromPos.Y - toPos.Y) + Mathf.Abs(fromPos.Z - toPos.Z);
	}

	public override float _EstimateCost(long fromId, long endId)
	{
		return _ComputeCost(fromId, endId);
	}

	public void Generate(Grid gridUsed, Mob mobUsed)
	{
		if (gridUsed is null) throw new Exception();

		MobAStar aStar = new();
		//List<Func<Vector3i, Vector3i, bool>> conditions = GetConnectionCondition();
		//Reserve space first to speed it up.
		aStar.ReserveSpace(
			gridUsed.Boundary.X * gridUsed.Boundary.Y * gridUsed.Boundary.Z
			);

		//Add the points
		foreach (var item in gridUsed.CellDictionary)
		{
			if (mobUsed.IsValidPositionToExist(gridUsed, item.Key))
				aStar.AddPoint(aStar.GetAvailablePointId(), item.Key.ToGVector3());
		}

		//Connect the points
		foreach (var pointId in aStar.GetPointIds())
		{
			Vector3i point = new(aStar.GetPointPosition(pointId));
			foreach (var pointTargetId in aStar.GetPointIds())
			{
				Vector3i pointTarget = new(aStar.GetPointPosition(pointTargetId));

				//If it can teleport, skip to connection.
				if (mobUsed.MovementModes.Contains(EMobMovementMode.TELEPORT)) goto perform_connection;

				//Otherwise, make sure it is adjacent
				else
				{
					bool adjacent = point.DistanceManhattanWithToleranceTo(
						pointTarget,
						new(0, gridUsed.Boundary.Y, 0)
						) == 1;
					bool canJumpThere = point.Y - pointTarget.Y <= mobUsed.Stats.GetStat(EStatName.JUMP);
					if (!adjacent || !canJumpThere) continue;
				} 

				perform_connection:
				aStar.ConnectPoints(pointId, pointTargetId);
			}
		}
	}	

	protected Func<Vector3i, Vector3i, bool> GetConnectionCondition(Mob mobUsed, EMobMovementMode mode)
	=> mode switch
		{
			EMobMovementMode.TELEPORT => (point, pointTarget) => true,
			_ => (point, pointTarget) => point.DistanceManhattanWithToleranceTo(
				pointTarget,
				new(0, (int)mobUsed.Stats.GetStat(EStatName.JUMP), 0)) < 1,
		};
}
