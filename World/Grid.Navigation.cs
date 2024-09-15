using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.World;
//public Navigation navigation = new();

public partial class Grid
{
    public Dictionary<IGridObject, AStar3D> ObjectToAStarDict = new();

    private Dictionary<IGridPosition, IGridObject> participants = new();
    //private UniqueList<IGridPosition> participants = new();

    public void NavAddObject(IGridObject grid_object)
    {
        ObjectToAStarDict[grid_object] = new();
    }
    public void NavRemoveObject(IGridObject grid_object)
    {
        ObjectToAStarDict.Remove(grid_object);
    }

    public bool NavIsGridObjectValid(IGridObject gridObject)
    {
        bool in_bounds = IsPositionInbounds(gridObject.GetPosition());
        bool exists_in_allowed_position = gridObject.IsValidPositionToExist(this, gridObject.GetPosition());

        return in_bounds && exists_in_allowed_position;
    }

    public List<Vector3i> NavGetPositionsInRange(IGridObject grid_object)
    {
        if (!NavIsGridObjectValid(grid_object)){throw new Exception("Not valid object.");}

        List<Vector3i> output = new();
        Vector3i obj_position = grid_object.GetPosition();
        int horizontal = grid_object.PathingGetHorizontalRange();
        int vertical = grid_object.PathingGetVerticalRange();

        int[] x_range = Enumerable.Range(-horizontal, horizontal*2+1).ToArray();
        int[] z_range = Enumerable.Range(-horizontal, horizontal*2+1).ToArray();
        int[] y_range = Enumerable.Range(-vertical, vertical*2+1).ToArray();

        foreach (var x in x_range)
        {
            foreach (var z in z_range)
            {
                foreach (var y in y_range)
                {
                    Vector3i vector = new Vector3i(x,y,z) + obj_position;
                    if (IsPositionInbounds(vector) 
                    && grid_object.PathingIsInRange(this, vector))
                    {
                        output.Add(vector);
                    }
                }
            }
        }
        return output;
    }

    public List<Vector3i> NavGetPathablePositions(IGridObject grid_object)
    {
        if (!NavIsGridObjectValid(grid_object)){throw new Exception("Not valid object.");}

        List<Vector3i> output = new();
        List<Vector3i> all_valid_to_exist = NavGetPositionsInRange(grid_object)
            .Where(x => grid_object.IsValidPositionToExist(this, x))
            .ToList();


        List<Vector3i> expand_candidates = new(){grid_object.GetPosition()};

        int iterations = 0;
        float starting_time = Time.GetTicksMsec();
        
        //Filter those that are valid to path from their origin.
        while (expand_candidates.Count != 0)
        {
            Vector3i curr_pos = expand_candidates.Last();
            expand_candidates.Remove(curr_pos);

            //Debug.Assert(all_valid_to_exist.Contains(curr_pos));

            if (output.Contains(curr_pos)){continue;}
            else {output.Add(curr_pos);}

            foreach (var candidate in all_valid_to_exist)
            {
                bool can_move_there = grid_object.IsValidMove(this, curr_pos, candidate);
                bool is_valid_pos = grid_object.IsValidPositionToExist(this, candidate);
                bool not_already_in_output = !output.Contains(candidate);
                if (can_move_there && is_valid_pos && not_already_in_output)
                {
                    expand_candidates.Add(item: candidate);         
                }

            }
            iterations ++;

        }
        Debug.WriteLine("Iterations for this pathable: " + iterations.ToString() + "\n" 
        + "Time spent (s): " + ((Time.GetTicksMsec() - starting_time)/1000).ToString());

        return output;

    }

    public static List<Vector3i> NavGetShortestPath(List<Vector3i> positions, Vector3i start, Vector3i target)
    {
        throw new NotImplementedException("Rework needed.");
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

}

