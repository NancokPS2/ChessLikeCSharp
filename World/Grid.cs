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

    protected Cell GetCell(Vector3i position)
    {
        Cell cell = Cell.Preset.Invalid;
        if (!CellDictionary.TryGetValue(position, out cell)){throw new Exception("Not found!");}
        return cell;
    }

    protected ICollection<Cell> GetCells()
    {
        return CellDictionary.Values.ToArray();
    }


    public Vector3i[] GetUsedPositions()
    {
        return CellDictionary.Keys.ToArray();
    }

    public List<Vector3i> GetShapeCube(Vector3i origin, uint max_distance)
    {
        List<Vector3i> output = new();

        int[] range = Enumerable.Range((int)-max_distance, (int)max_distance*2+1).ToArray();

        foreach (var x in range)
        {
            foreach (var z in range)
            {
                foreach (var y in range)
                {
                    Vector3i vector = new Vector3i(x,y,z) + origin;
                    output.Add(vector);
                }
            }
        }
        output = output.Where(x => IsPositionInbounds(x)).ToList();
        return output;   
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
    public bool IsFlagInPosition(Vector3i position, ECellFlag flag)
    {
        Cell cell = GetCell(position);
        if(cell == Cell.Preset.Invalid)
        {
            return false;
        }

        return flag == ECellFlag.UNKNOWN || cell.Flags.Contains(flag);
    }
    public bool IsFlagInPosition(Vector3i position, ICollection<ECellFlag> flags)
    {
        foreach (ECellFlag flag in flags)
        {
            if(!IsFlagInPosition(position, flag))
            {
                return false;
            }
            
        }
        return true;
    }

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
        public List<ECellFlag> BlacklistedFlags;

        public FloodFillParameters(List<ECellFlag> blacklisted_flags, int vertical_tolerance = 0)
        {
            this.BlacklistedFlags = blacklisted_flags;
            this.VerticalTolerance = vertical_tolerance;
        }
    }
}
