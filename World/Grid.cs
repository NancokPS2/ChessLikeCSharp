using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

public partial class Grid : Resource
{
    public static Godot.Vector3 CellSize = new(1,1,1);
    public Vector3i Boundary = new(10, 10, 10);

    public Dictionary<Vector3i, GridCell> CellDictionary = new();

    public Grid()
    {
    }
    #region Cells
    public void SetCell(Vector3i position, GridCell cell)
    {
        CellDictionary[position] = cell;
    }

    public GridCell GetCell(Vector3i position)
    {
        GridCell cell = GridCell.Preset.Invalid;
        if (!CellDictionary.TryGetValue(position, out cell)){throw new Exception("Not found!");}
        return cell;
    }

    public ICollection<GridCell> GetCells()
    {
        return CellDictionary.Values.ToArray();
    }
    #endregion

    #region Positions
    //public static Godot.Vector3 MapToReal(Vector3i mapPos) => mapPos * CellSize;
    
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
    #endregion

    #region Checks
    public bool IsPositionInbounds(Vector3i position)
    {

        if (position.X < 0 || position.Y < 0 || position.Z < 0)
        {
            return false;
        }
        if (position.X >= Boundary.X || position.Y >= Boundary.Y || position.Z >= Boundary.Z)
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
        GridCell cell = GetCell(position);
        if(cell == GridCell.Preset.Invalid)
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
    #endregion
    
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
