using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Mob : IGridObject
{
    public Vector3i GetPosition()
    {
        return Position;
    }

    public bool IsValidMove(Grid grid, Vector3i from, Vector3i to)
    {
        int distance;
        switch (MovementModes)
        {
            case EMobMovementMode.GROUNDED:
                distance = from.DistanceManhattanWithToleranceTo(
                        to, 
                        new(0, (int)Stats.GetStat(EStatName.JUMP), 0)
                    );
                return distance <= 1;
            
            default: throw new NotImplementedException();
                distance = from.DistanceManhattanWithToleranceTo(
                        to, 
                        new(0, (int)Stats.GetStat(EStatName.MOVEMENT), 0)
                    );
                return distance <= 1;
        }
        
    }

    public bool IsValidPositionToExist(Grid grid, Vector3i position)
    {
        bool canExist =
			CellExistWhitelist.All(x => grid.IsFlagInPosition(position, x))
			&& CellExistBlacklist.All(x => !grid.IsFlagInPosition(position, x));
        bool canStandOn = 
			CellStandWhitelist.All(x => grid.IsFlagInPosition(position + Vector3i.DOWN, x))
			&& CellStandBlacklist.All(x => !grid.IsFlagInPosition(position + Vector3i.DOWN, x));
        return canExist && canStandOn;
    }

    public int PathingGetHorizontalRange()
    {
        return (int)Stats.GetStat(EStatName.MOVEMENT);
    }

    public int PathingGetVerticalRange()
    {
        return (int)Stats.GetStat(EStatName.JUMP);
    }

    public bool PathingIsInRange(Grid grid, Vector3i position)
    {
       return GetPosition()
       .DistanceManhattanWithToleranceTo(
        position, 
        new(0, PathingGetVerticalRange(), 0)) <= PathingGetHorizontalRange();
    }
}
