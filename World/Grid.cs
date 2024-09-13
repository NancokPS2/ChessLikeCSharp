using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

public partial class Grid
{
    public Vector3i Boundary = new(10,10,10);
    private AStarGridPathing AStarPathing;

    public Dictionary<Vector3i, Cell> CellDictionary = new();

    public Grid()
    {
        AStarPathing = new(this);
    }

    public void SetCell(Vector3i position, Cell cell)
    {
        CellDictionary[position] = cell;
    }

    public Cell GetCell(Vector3i position)
    {
        Cell cell = Cell.Preset.Invalid;
        if (!CellDictionary.TryGetValue(position, out cell)){throw new Exception("Not found!");}
        return cell;
    }

    public Cell[] GetCells()
    {
        return CellDictionary.Values.ToArray();
    }


    public Vector3i[] GetUsedPositions()
    {
        return CellDictionary.Keys.ToArray();
    }

/*     public Vector3i[] GetUsedPositionsInThisColumn(Vector3i pos_in_column)
    {
        List<Vector3i> output = new();
        Vector3i[] used_positions = GetUsedPositions();
        Vector3i position_curr = new(pos_in_column.X, 0, pos_in_column.Z);

        if (!IsPositionInbounds(position_curr)){throw new Exception("The bottom should exist!");}

        foreach (Vector3i item in cells_dictionary.Keys)
        {
            bool inbounds = IsPositionInbounds(position_curr);
            bool used = used_positions.Contains(position_curr);
            if (inbounds && used)
            {
                output.Add(position_curr);
            }
            else
            {
                Debug.Print("Cut off column at " + position_curr.ToString() + inbounds.ToString() + used.ToString());
                break;
            }
            position_curr += Vector3i.UP;
        }
        
        return output.ToArray();
    } */

    public bool IsPositionInbounds(Vector3i position)
    {
        
        if(position.X < 0 || position.Y < 0 || position.Z < 0)
        {
            return false;
        }
        if(position.X >= Boundary.X || position.Y >= Boundary.Y || position.Z >= Boundary.Z)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the flag is present at the position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <param name="flag">The flag to check, Cell.Flag.UNKNOWN it will always return true.</param>
    /// <returns>If the flag qualifies as being in the position.</returns>
    public bool IsFlagInPosition(Vector3i position, CellFlag flag)
    {
        Cell cell = GetCell(position);
        if(cell == Cell.Preset.Invalid)
        {
            return false;
        }

        return flag == CellFlag.UNKNOWN || cell.flags.Contains(flag);
    }
    public bool IsFlagInPosition(Vector3i position, CellFlag[] flags)
    {
        foreach (CellFlag flag in flags)
        {
            if(!IsFlagInPosition(position, flag))
            {
                return false;
            }
            
        }
        return true;
    }
    public bool IsFlagInPosition(Vector3i position, List<CellFlag> flags)
    {
        foreach (CellFlag flag in flags)
        {
            if(!IsFlagInPosition(position, flag))
            {
                return false;
            }
            
        }
        return true;
    }

    /// <summary>
    /// Gets all cells within a certain distance as another.
    /// </summary>
    /// <param name="origin">From where to start checking.</param>
    /// <param name="distance">How far from origin to reach.</param>
    /// <param name="step_filters">
    /// Called for every position checked, it returns wheter or not it can be in the final result.
    /// Also if it is valid to keep searching from said location.
    /// True = allow.
    /// </param>
    /// <returns></returns>
    public List<Vector3i> GetFloodFill(Vector3i origin, int max_steps, List<Func<Vector3i,bool>>? step_filters = null)
    {
        
        List<Vector3i> output = new();
        List<Vector3i> to_expand = new(new Vector3i[]{origin});

        while (!(to_expand.Count > 0))
        {
            Vector3i curr_position = to_expand.Last();
            to_expand.Remove(curr_position);

            //Skip if already in the output
            if(output.Contains(curr_position)){continue;}

            //Skip if too far from the start (Not necessarily wanted.)
            //if(curr_position.DistanceManhattanTo(origin) > max_steps){continue;}

            //Skip if the step filter returns false.
            if(step_filters is not null)
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
/* 
    public List<Vector3i> GetFloodFill(Vector3i start, int size, Func<Vector3i,bool>[]? filters = null)
    {
        filters ??= new[]{ this.filters.None};
        
        List<Vector3i> output = new();
        List<Vector3i> to_expand = new(new Vector3i[]{start});

        while (!(to_expand.Count > 0))
        {
            Vector3i curr_position = to_expand.Last();
            to_expand.Remove(curr_position);

            //Skip if already in the output
            if(output.Contains(curr_position)){continue;}

            //Skip if any of the filters returns false
            if (filters.Any(x => !x(curr_position)))
            {
                continue;
            }

            //Skip if too far from the start
            if(curr_position.DistanceManhattanTo(start) > size){continue;}

            output.Add(curr_position);

            foreach (Vector3i direction in Vector3i.DIRECTIONS)
            {
                Vector3i adj_position = direction + curr_position;

                //Skip if already in the output
                if(output.Contains(adj_position)){continue;}

                to_expand.Add(adj_position);
            }
        }
        return output;

    }
 */
    public static class StepFilterPresets
    {
        public static bool Always(Vector3i vector)
        {
            return true;
        }
    }
    public struct FloodFillParameters
    {
        public int VerticalTolerance;
        public List<CellFlag> BlacklistedFlags;

        public FloodFillParameters(List<CellFlag> blacklisted_flags, int vertical_tolerance = 0)
        {
            this.BlacklistedFlags = blacklisted_flags;
            this.VerticalTolerance = vertical_tolerance;
        }
    }
}
