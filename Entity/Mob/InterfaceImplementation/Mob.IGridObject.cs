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
        switch (MovementMode)
        {
            case EMovementMode.WALK:
                distance = from.DistanceManhattanWithToleranceTo(
                        to, 
                        new(0, (int)Stats.GetValue(StatName.JUMP), 0)
                    );
                return distance <= 1;
            
            default:
                distance = from.DistanceManhattanWithToleranceTo(
                        to, 
                        new(0, (int)Stats.GetValue(StatName.MOVEMENT), 0)
                    );
                return distance <= 1;
        }
        
    }

    public bool IsValidPositionToExist(Grid grid, Vector3i position)
    {
        bool can_exist;
        bool can_stand_on;
        switch (MovementMode)
        {
            case EMovementMode.WALK:
                can_exist = grid.IsFlagInPosition(position, ECellFlag.AIR);

                can_stand_on = grid.IsPositionInbounds(position + Vector3i.DOWN) 
                && grid.IsFlagInPosition(position + Vector3i.DOWN, ECellFlag.SOLID);
                
                return can_exist && can_stand_on;
                
            default:
                can_exist = grid.IsFlagInPosition(position, ECellFlag.AIR);
                can_stand_on = grid.IsFlagInPosition(position + Vector3i.DOWN, ECellFlag.SOLID);
                return can_exist && can_stand_on;
        }
        
    }

    public int PathingGetHorizontalRange()
    {
        return (int)Stats.GetValue(StatName.MOVEMENT);
    }

    public int PathingGetVerticalRange()
    {
        return (int)Stats.GetValue(StatName.JUMP);
    }

    public bool PathingIsInRange(Grid grid, Vector3i position)
    {
       return GetPosition()
       .DistanceManhattanWithToleranceTo(
        position, 
        new(0, PathingGetVerticalRange(), 0)) <= PathingGetHorizontalRange();
    }
}
