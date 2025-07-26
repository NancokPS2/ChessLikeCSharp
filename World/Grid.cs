using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ChessLike.Entity;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

[GlobalClass, Tool]
public partial class Grid : Resource
{
    public static Godot.Vector3 CellSize = new(1, 1, 1);

    public Vector3i Boundary = new(10, 10, 10);
    [Export]
    private Vector3I boundary
    {
        set => Boundary = new(value);
        get => Boundary.ToGVector3I();
    }

    public Dictionary<Vector3i, GridCell> CellDictionary = new();
    [Export]
    private Godot.Collections.Dictionary<Godot.Vector3I, GridCell> cellDictionary
    {
        set
        {
            Dictionary<Vector3i, GridCell> input = new();
            foreach (var item in value)
            {
                input[new(item.Key)] = item.Value;
            }
            CellDictionary = input;
        }
        get
        {
            Godot.Collections.Dictionary<Godot.Vector3I, GridCell> output = new();
            foreach (var item in CellDictionary)
            {
                output[item.Key] = item.Value;
            }
            return output;
        }
    }

    public Dictionary<Vector3i, List<PackedScene>> Decorations;

    public Grid()
    {
    }
    #region Cells
    public void SetCell(Vector3i position, GridCell cell)
    {
        if (!IsPositionInbounds(position)) throw new Exception($"Position {position} out of bounds.");
        CellDictionary[position] = cell;
    }

	public void FillCellWhere(GridCell cell, Func<Vector3i, bool> conditionFunc)
	{
		foreach (var item in Vector3i.CreateBox(Boundary))
		{
			bool condition = conditionFunc(item);
			if (condition)
				SetCell(item, cell);
		}		
	}

    public void FillCell(GridCell cell, bool emptyOnly)
	{
		for (int x = 0; x < Boundary.X; x++)
		{
			for (int y = 0; y < Boundary.Y; y++)
			{
				for (int z = 0; z < Boundary.Z; z++)
				{
					Vector3i vector = new(x, y, z);
					if (emptyOnly && HasCell(vector)) continue;
					SetCell(vector, cell);
				}
			}

		}
	}

    public GridCell GetCell(Vector3i position)
    {
        GridCell cell = GridCell.Preset.Invalid;
        if (!CellDictionary.TryGetValue(position, out cell)) { throw new Exception("Not found!"); }
        return cell;
    }

    public bool HasCell(Vector3i position)
        => CellDictionary.ContainsKey(position);

    public ICollection<GridCell> GetCells()
    {
        return CellDictionary.Values.ToArray();
    }

    /// <summary>
    /// Makes a list of all unique GridCells
    /// </summary>
    /// <returns></returns>
    public List<GridCell> GetGridCells()
    {
        List<GridCell> output = new();
        foreach (var item in CellDictionary.Values)
        {
            if (!output.Contains(item)) output.Add(item);
        }
        return output;
    }

    public void ClearCells() => CellDictionary.Clear();
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

        int[] range = Enumerable.Range((int)-max_distance, (int)max_distance * 2 + 1).ToArray();

        foreach (var x in range)
        {
            foreach (var z in range)
            {
                foreach (var y in range)
                {
                    Vector3i vector = new Vector3i(x, y, z) + origin;
                    output.Add(vector);
                }
            }
        }
        output = output.Where(x => IsPositionInbounds(x)).ToList();
        return output;
    }
    #endregion

    public EFaction GetFactionSpawn(Vector3i pos)
        => GetCell(pos).FactionSpawn;

    #region Checks
    public bool IsPositionInbounds(Vector3i position)
        => IsPositionInbounds(position, Boundary);
    public static bool IsPositionInbounds(Vector3i position, Vector3i boundary)
    {

        if (position.X < 0 || position.Y < 0 || position.Z < 0)
        {
            return false;
        }
        else if (position.X >= boundary.X || position.Y >= boundary.Y || position.Z >= boundary.Z)
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
        if (cell == GridCell.Preset.Invalid)
        {
            return false;
        }

        return flag == ECellFlag.UNKNOWN || cell.Flags.Contains(flag);
    }
    public bool IsFlagInPosition(Vector3i position, ICollection<ECellFlag> flags)
    {
        foreach (ECellFlag flag in flags)
        {
            if (!IsFlagInPosition(position, flag))
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
