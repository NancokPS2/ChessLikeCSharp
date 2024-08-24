using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IGridPosition
{
    const float DISTANCE_THRESHOLD = 0.1f;
    
    public List<Vector3i> QueuedGridPositions {get;set;}
    public Vector3i Position {get; set;}
    public Vector3 FloatPosition {get; set;}
    public float Speed {get; set;}

/* 
    //public void AdvanceToLocation(Vector3i location, float delta);
    {
        Vector3 target = location.ToVector3();

        Vector3 movement = Vector3.Lerp(Position, target, delta);

        FloatPosition += movement;
    } 

    public void AdvanceInDirection(Vector3i direction)
    {
        if (!direction.IsNormalized())
        {
            throw new ArgumentException("Must be normalized.");
        }
        Position += direction;
    }

    public void AddQueuedLocation(Vector3i location)
    {
        QueuedGridPositions.Add(location);
    }
    public void ClearQueuedLocation()
    {
        QueuedGridPositions.Clear();
    }

    public static int GetDistance(Vector3i point_a, Vector3i point_b)
    {
        return point_a.DistanceManhattanTo(point_b);
    }

    public int GetDistance(Vector3i point)
    {
        return Position.DistanceManhattanTo(point);
    }
    */

}
public static class IGridPositionExtensionMethods
{
    public static void AdvanceToLocation(this IGridPosition @this, Vector3i location, float delta)
    {
        Vector3 target = location.ToVector3();

        Vector3 movement = Vector3.Lerp(@this.Position, target, delta);

        @this.FloatPosition += movement;
    }

    public static void AdvanceInDirection(this IGridPosition @this, Vector3i direction)
    {
        if (!direction.IsNormalized())
        {
            throw new ArgumentException("Must be normalized.");
        }
        @this.Position += direction;
    }

    public static void AddQueuedLocation(this IGridPosition @this, Vector3i location)
    {
        @this.QueuedGridPositions.Add(location);
    }
    public static void ClearQueuedLocation(this IGridPosition @this)
    {
        @this.QueuedGridPositions.Clear();
    }

    public static int GetDistance(Vector3i point_a, Vector3i point_b)
    {
        return point_a.DistanceManhattanTo(point_b);
    }

    public static int GetDistance(this IGridPosition @this, Vector3i point)
    {
        return @this.Position.DistanceManhattanTo(point);
    }
}