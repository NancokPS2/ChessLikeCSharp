using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using Godot;

namespace ChessLike.World;

public class Grid
{
    readonly Filter filters;
    readonly NavigationController navigation_controller;

    public Vector3i boundary = new(10,10,10);

    Dictionary<Vector3i, Cell> cells = new();

    public Grid()
    {
        this.filters = new Filter(this);
        this.navigation_controller = new NavigationController(this);
    }

    public void SetCell(Vector3i position, Cell cell)
    {
        cells[position] = cell;
    }

    public Cell GetCell(Vector3i position)
    {
        Cell cell = Cell.Preset.Invalid;
        cells.TryGetValue(position, out cell);
        return cell;
    }

    public Cell[] GetCells()
    {
        return cells.Values.ToArray();
    }

    public Vector3i[] GetUsedPositions()
    {
        return cells.Keys.ToArray();
    }

    /// <summary>
    /// Gets the nearest position for something to stand on. If the selected position cannot be used, find the nearest one upwards (dig upwards). Otherwise go downwards (land).
    /// </summary>
    /// <returns>A non-SOLID cell.</returns>
    public Vector3i GetNearestSurfaceVertically(Vector3i position, Cell.Flag can_stand_on = Cell.Flag.SOLID, Cell.Flag can_exist_on = Cell.Flag.AIR)
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

    public bool CanStandHere(Vector3i position, Cell.Flag flag_below_required, Cell.Flag flag_exist_allowed)
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
        if(position.X > boundary.X || position.Y > boundary.Y || position.Z > boundary.Z)
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
    public bool IsFlagInPosition(Vector3i position, Cell.Flag flag)
    {
        Cell cell = GetCell(position);
        if(cell == Cell.Preset.Invalid)
        {
            return false;
        }

        return flag == Cell.Flag.UNKNOWN || cell.flags.Contains(flag);
    }
    public bool IsFlagInPosition(Vector3i position, Cell.Flag[] flags)
    {
        foreach (Cell.Flag flag in flags)
        {
            if(!IsFlagInPosition(position, flag))
            {
                return false;
            }
            
        }
        return true;
    }

    public List<Vector3i> GetCellsWithinDistance(Vector3i origin, int distance)
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

    public class Generator
    {
        public static Grid GenerateFlat(Vector3i size)
        {
            Grid output = new();
            int[] X = Enumerable.Range(0, (int)size.X).ToArray();
            int[] Y = Enumerable.Range(0, (int)size.Y).ToArray();
            int[] Z = Enumerable.Range(0, (int)size.Z).ToArray();
            foreach (int ind_x in X)
            {
                foreach (int ind_y in X)
                {
                    foreach (int ind_z in X)
                    {
                        if (ind_y < 1)
                        {
                            output.SetCell(new Vector3i(ind_x,ind_y,ind_z), Cell.Preset.Floor);
                        }else
                        {
                            output.SetCell(new Vector3i(ind_x,ind_y,ind_z), Cell.Preset.Air);
                        }
                    }
                }

            }

            return output;
        }
    }
}
