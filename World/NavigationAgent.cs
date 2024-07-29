using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ChessLike.World;

public class NavigationAgent
{
    const int QUEUE_INVALID_INDEX = -1;
    public Vector3i position;
    public float movement_speed;
    Vector3 position_float;

    List<Vector3i> moves_queued = new();
    int queue_index = 0;


    public void UpdateFloatMovement()
    {
        Vector3i curr_target = QueueGetMove();
        Vector3 float_target = (Vector3)curr_target;
        Vector3 float_difference = float_target - position_float;
        position_float += float_difference * movement_speed;
    }

    public void QueueMove(Vector3i vector)
    {
        moves_queued.Add(vector);
    }

    private void QueueIndexUpdate()
    {
        //Advance if the target was reached.
        Vector3i curr_position_target = moves_queued[queue_index];
        if (position == curr_position_target)
        {
            queue_index += 1;
        }

        //If it reached the end of the queue.
        if(queue_index >= moves_queued.Count)
        {
            //If the queue is empty, become invalid, otherwise return to 0.
            queue_index = moves_queued.Count == 0 ? QUEUE_INVALID_INDEX : 0;
        }

    }

    private Vector3i QueueGetMove()
    {
        QueueIndexUpdate();
        if (moves_queued.Count == 0)
        {
            return Vector3i.ZERO;
        }
        return moves_queued[queue_index];
    }


    public Vector3 GetFloatLocation()
    {
        return position_float;
    }
}
