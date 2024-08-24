using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

public partial class Grid
{
    public Vector3i boundary = new(10,10,10);

    public Dictionary<Vector3i, Cell> cells_dictionary = new();

    public void SetCell(Vector3i position, Cell cell)
    {
        cells_dictionary[position] = cell;
    }

    public Cell GetCell(Vector3i position)
    {
        Cell cell = Cell.Preset.Invalid;
        cells_dictionary.TryGetValue(position, out cell);
        return cell;
    }

    public Cell[] GetCells()
    {
        return cells_dictionary.Values.ToArray();
    }

    public Vector3i[] GetUsedPositions()
    {
        return cells_dictionary.Keys.ToArray();
    }

    /// <summary>
    /// Gets the nearest position for something to stand on. If the selected position cannot be used, find the nearest one upwards (dig upwards). Otherwise go downwards (land).
    /// </summary>
    /// <returns>A non-SOLID cell.</returns>
    public Vector3i GetNearestSurfaceVertically(Vector3i position, CellFlag can_stand_on = CellFlag.SOLID, CellFlag can_exist_on = CellFlag.AIR)
    {
        Vector3i advance_dir = !IsFlagInPosition(position, can_exist_on) ? Vector3i.UP : Vector3i.DOWN;
        Vector3i curr_pos = position;
        bool turned = false;
        
        move:
        while(IsPositionInbounds(curr_pos))
        {
            if(CanStandHere(curr_pos, can_stand_on, can_exist_on))
            {
                return curr_pos;
            }

            curr_pos = curr_pos + advance_dir;
        }

        //Turn around if nothing was found in that direction.
        if (!turned)
        {
            turned = true;
            advance_dir *= -1;
            goto move;
        }

        //If it already turned, there is a full column here. Return the invalid Vector3i.
        return Vector3i.INVALID;

    }

    public bool CanStandHere(Vector3i position, CellFlag flag_below_required, CellFlag flag_exist_allowed)
    {
        return 
            IsFlagInPosition(position + Vector3i.DOWN, flag_below_required) 
            && IsFlagInPosition(position, flag_exist_allowed)
        ;

    }

    public bool IsPositionInbounds(Vector3i position)
    {
        
        if(position.X < 0 || position.Y < 0 || position.Z < 0)
        {
            return false;
        }
        if(position.X >= boundary.X || position.Y >= boundary.Y || position.Z >= boundary.Z)
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
