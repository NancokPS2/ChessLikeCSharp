using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public partial class Grid
{
    private UniqueList<Vector3i> flood_to_expand = new();
    private List<Vector3i> flood_output = new();
    private Vector3i flood_current_origin;

    public void FloodStart(Vector3i origin)
    {
        //flood_output = new(){origin};
        flood_to_expand = new(){origin};

        flood_current_origin = origin;
        FloodExpandToAdjacent();
    }
    /// <summary>
    /// Replaces to content of flood_to_expand with adjacent positions.
    /// </summary>
    public void FloodExpandToAdjacent()
    {
        List<Vector3i> new_flood_to_expand = new(flood_to_expand);

        foreach (Vector3i position in new_flood_to_expand)
        {
            //Pass this position to the output.
            flood_output.Add(position);
            flood_to_expand.Remove(position);

            foreach (Vector3i direction in Vector3i.DIRECTIONS)
            {
                Vector3i considered = position + direction;
                //Add it if it is not in the output.
                if(!flood_output.Contains(considered))
                {
                    flood_to_expand.Add(considered, false);
                }
            }
        }

    }
    /// <summary>
    /// Removes all positions that don't pass the filter.
    /// </summary>
    /// <param name="filter"></param> <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    public void FloodApplyFilterToExpand( Func<Vector3i,bool> filter)
    {
        foreach (Vector3i position in flood_to_expand)
        {
            if (!filter(position))
            {
                flood_to_expand.Remove(position);
            }
        }
    }
    public List<Vector3i> FloodGetToExpand()
    {
        return flood_to_expand;
    }
    public void FloodAddToExpand(Vector3i position)
    {
        flood_to_expand.Add(obj: position);
    }
    public void FloodRemoveToExpand(Vector3i position)
    {
        flood_to_expand.Remove(position);
    }

    public List<Vector3i> FloodGetResult(bool finish = true)
    {
        List<Vector3i> output = flood_output;

        FloodStart(new());

        return output;
    }

    public List<Vector3i> TEST(Vector3i origin, int distance, List<Func<Vector3i,bool>>? step_filters = null)
    {
        
        List<Vector3i> output = new();
        List<Vector3i> to_expand = new(new Vector3i[]{origin});

        while (!(to_expand.Count > 0))
        {
            Vector3i curr_position = to_expand.Last();
            to_expand.Remove(curr_position);

            //Skip if already in the output
            if(output.Contains(curr_position)){continue;}

            //Skip if too far from the start
            if(curr_position.DistanceManhattanTo(origin) > distance){continue;}

            //Skip if the step filter returns false.
            if(step_filters != null)
            {
                foreach (Func<Vector3i,bool> filter in step_filters)
                {
                    if(!filter(curr_position)){goto skip;}
                }

            }

            output.Add(curr_position);

            foreach (Vector3i direction in Vector3i.DIRECTIONS)
            {
                Vector3i adj_position = direction + curr_position;

                //Skip if already in the output
                if(output.Contains(adj_position)){continue;}

                to_expand.Add(adj_position);
            }

            skip:
            continue;
            
        }
        return output;
    }
    
}
