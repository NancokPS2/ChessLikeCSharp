using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sprache;

namespace ChessLike.Shared;

public interface IPosition
{
    
    public Vector3i Position {get; set;}
    public float Speed {get; set;}
    public float GetCellMovementCost(World.Cell cell);

    public static int GetDistance(Vector3i point_a, Vector3i point_b)
    {
        return point_a.DistanceManhattanTo(point_b);
    }

    public int GetDistance(Vector3i point)
    {
        return Position.DistanceManhattanTo(point);
    }

}