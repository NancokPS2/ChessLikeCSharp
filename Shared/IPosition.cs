using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sprache;

namespace ChessLike.Shared;

public interface IGridPosition
{
    const float DISTANCE_THRESHOLD = 0.1f;
    
    public List<Vector3i> QueuedGridPositions {get;set;}
    public Vector3i GridPosition {get; set;}
    public Vector3 Position {get; set;}
    public float Speed {get; set;}

    public void AdvanceToLocation(Vector3i location, float delta)
    {
        Vector3 target = location.ToVector3();

        Vector3 movement = Vector3.Lerp(GridPosition, target, delta);

        Position += movement;
    }

    public void AdvanceInDirection(Vector3i direction)
    {
        if (!direction.IsNormalized())
        {
            throw new ArgumentException("Must be normalized.");
        }
        GridPosition += direction;
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
        return GridPosition.DistanceManhattanTo(point);
    }

}