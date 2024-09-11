using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.World;
//public Navigation navigation = new();
public class Navigation
{
    public Grid grid;
    public AStar3D pathing;
    public Navigation(Grid grid)
    {
        this.grid = grid;
    }

    private Dictionary<IGridPosition, NavigationAgent> participants = new();
    //private UniqueList<IGridPosition> participants = new();

    public NavigationAgent AddAgent(IGridPosition participant)
    {
        participants[participant] = new(participant);
        return GetAgent(participant);
    }
    public void RemoveAgent(IGridPosition participant)
    {
        participants.Remove(participant);
    }
    public NavigationAgent GetAgent(IGridPosition participant)
    {
        return participants[participant];
    }
    /// <summary>
    /// The ground is considered as any cell that the agent can stand on. This measures the distance from the agent to the ground.
    /// </summary>
    /// <param name="agent"></param>
    /// <returns>The distance from the ground. -1 if there is no ground.</returns>
    public int GetDistanceFromGround(NavigationAgent agent, Vector3i position)
    {
        Vector3i position_going_down = position;
        while(grid.IsPositionInbounds(position_going_down))
        {
            if (grid.IsFlagInPosition(position_going_down, agent.flags_to_stand))
            {
                return position.DistanceManhattanTo(position_going_down);
            }
            position_going_down += Vector3i.DOWN;
        }
        return -1;
    }

    public List<Vector3i> GetPositionsInRange(NavigationAgent agent, int max_distance)
    {
        List<Vector3i> output = new();

        grid.FloodStart(agent.user.Position);

        //Perform the expansion.
        for (int step = 0; step < max_distance; step++)
        {
            grid.FloodExpandToAdjacent();

            List<Vector3i> aux_expand = new(grid.FloodGetToExpand());
            //Apply logic to each position
            foreach (Vector3i expand_pos in aux_expand)
            {
                if (!grid.IsPositionInbounds(expand_pos))
                {
                    grid.FloodRemoveToExpand(expand_pos);
                    continue;
                }

                //If the agent cannot exist in the given spot... 
                if ( !grid.IsFlagInPosition(expand_pos, agent.flags_to_exist) )
                {
                    //Remove it.
                    grid.FloodRemoveToExpand(expand_pos);

                    //Then try to find a place to jump to that is above this one.
                    Vector3i pos_curr = expand_pos;
                    Vector3i? pos_valid = null;

                    //Start checking every position upwards.
                    for (int i = 0; i < agent.jump_altitude; i++)
                    {
                        pos_curr += Vector3i.UP;
                        if ( AgentCanExist(agent, expand_pos) )
                        {
                            pos_valid = pos_curr;
                        }
                    }
                    //If a pos_valid was defined, add it to the ones to expand.
                    if(pos_valid is Vector3i valid) {grid.FloodAddToExpand(valid);};
                }

                //If it has flight, the height may not surpass its capacity. In the ground, the distance is at least 1.
                bool can_fly_here = GetDistanceFromGround(agent, expand_pos) <= agent.flight_altitude + 1;
                
                //If it can't fly nor stand here, remove it.
                if(!AgentCanStand(agent, expand_pos) && !can_fly_here) 
                {
                    grid.FloodRemoveToExpand(expand_pos);
                }
            }
        }

        output = grid.FloodGetResult();
        return output;
    }

    public static List<Vector3i> GetShortestPath(List<Vector3i> positions, Vector3i start, Vector3i target)
    {
        if (!positions.Contains(start))
        {
            throw new Exception("start must be included in the positions.");
        }
        if (!positions.Contains(target))
        {
            Debug.WriteLine("Cannot get path since the target is not in the allowed positions.");
            return new();
        }
        List<Vector3i> output = new();

        //AStar initialization
        Dictionary<Vector3, long> vector_to_point = new();
        AStar3D astar = new();
        astar.ReserveSpace(positions.Count());

        //Add the points
        foreach (Vector3i pos in positions)
        {   
            long id = astar.GetAvailablePointId();
            astar.AddPoint(astar.GetAvailablePointId(), pos.ToGVector3());
            vector_to_point[pos.ToGVector3()] = id;
        }

        //Connect them
        foreach (long point_id in astar.GetPointIds())
        {
            Vector3 pos = astar.GetPointPosition(point_id);
            foreach (Vector3i dir in Vector3i.DIRECTIONS)
            {

                if (vector_to_point.Keys.Contains(pos + dir.ToGVector3()))
                {
                    long target_point = vector_to_point[pos + dir.ToGVector3()];
                    astar.ConnectPoints(
                        point_id,
                        target_point
                    );
                }
            }

        }

        Vector3[]? path = astar.GetPointPath(
            vector_to_point[start.ToGVector3()],
            vector_to_point[target.ToGVector3()]
        );
        foreach (Vector3 pos in path)
        {
            output.Add(new(pos));
        }
        return output;
    }
/* 
    public static List<Vector3i> GetShortestPath(List<Vector3i> positions, Vector3i start, Vector3i target)
    {
        UniqueList<Vector3i> output = new();
        UniqueList<Vector3i> output_bl = new();
        Vector3i pos_curr = start;

        while (pos_curr.DistanceManhattanTo(target) > 0)
        {
            //Get the valid positions
            List<Vector3i> valid_dirs = new();
            foreach (Vector3i dir in Vector3i.DIRECTIONS)
            {
                if (positions.Contains(pos_curr + dir))
                {
                    valid_dirs.Add(pos_curr + dir);
                } 
            }

            //Try to advance towards the target
            Vector3i closer_position = pos_curr + pos_curr.GetDirectionNormalizedTo(target, valid_dirs.ToArray());

            if (positions.Contains(closer_position) && !output_bl.Contains(closer_position))
            {
                output.Add(closer_position);

            }
            // If it cannot advance there, look for another direction route.
        }

        return output;

    }
 */
    public bool AgentCanExist(NavigationAgent agent, Vector3i position)
    {
        return grid.IsFlagInPosition(position, agent.flags_to_exist);
    }
    public bool AgentCanStand(NavigationAgent agent, Vector3i position)
    {
        if(grid.IsPositionInbounds(position))
        {
            return false;
        } else
        {
            return grid.IsFlagInPosition(position + Vector3i.DOWN, agent.flags_to_exist);
        }
    }

}
public class NavigationAgent
{
    public IGridPosition user;
    public int jump_altitude = 2;
    public int flight_altitude = 0;
    public List<CellFlag> flags_to_stand = new(){CellFlag.SOLID};
    public List<CellFlag> flags_to_exist = new(){CellFlag.AIR};

    public NavigationAgent(IGridPosition user)
    {
        this.user = user;
    }

    public NavigationAgent SetFlightHeight(int height)
    {
        flight_altitude = height;
        return this;
    }

    public NavigationAgent SetFlagsToStand(List<CellFlag> flags)
    {
        flags_to_stand = flags;
        return this;
    }    
    public NavigationAgent SetFlagsToExist(List<CellFlag> flags)
    {
        flags_to_exist = flags;
        return this;
    }
}

