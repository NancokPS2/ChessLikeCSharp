using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer;
using Godot;

namespace ChessLike.GenericGridStorage;

public class Grid
{
	protected Dictionary<Vector3i, Cell> CellDict = new();
	public Godot.Vector3 CellSize { protected set; get; }

	public Grid()
	{
		CellSize = new(1, 1, 1);
	}

	public Grid(Godot.Vector3 cellSize, Vector3i initialSize)
	{
		CellSize = cellSize;
		Resize(initialSize);
	}

	public List<Cell> CellGetAll()
		=> new(CellDict.Values);

	public void Resize(Vector3i size)
	{
		//Remove those outside the boundaries
		foreach (var pos in new Dictionary<Vector3i, Cell>(CellDict).Keys)
		{
			if (!IsPositionInside(pos, size))
				CellRemove(pos);
		}
		//Fill any that is empty.
		foreach (var pos in size.GetVolume())
		{
			if (!CellExists(pos))
				CellSet(pos, new());
		}
	}

	public void CellSet(Vector3i pos, Cell cell)
	{
		CellDict[pos] = cell;
		cell.SetPosition(pos);
	}

	public void CellSetInvalidated(Vector3i pos)
		=> CellSet(pos, Cell.INVALID);

	public void CellSetIntersect(Vector3i pos, bool intersect)
	{
		CellGet(pos).SetIntersect(intersect);
	}

	public void CellSetIntersect(Vector3i[] positions, bool intersect)
	{
		foreach(var pos in positions) 
		{
			CellGet(pos).SetIntersect(intersect);
		}
	}

	public Cell CellGet(Vector3i pos)
	{
		Cell output = CellDict[pos];
		return output;
	}

	protected bool CellRemove(Vector3i pos)
		=> CellDict.Remove(pos);

	public bool CellExists(Vector3i pos)
		=> CellDict.ContainsKey(pos) && !CellDict[pos].IsValid();

	public bool CellIsValid(Vector3i pos)
		=> CellExists(pos) && CellGet(pos).IsValid();

	public bool IsPositionInside(Vector3i pos, Vector3i size)
		=> pos.X <= size.X && pos.Y < size.Y && pos.Z < size.Z;

	public Vector3i[] GetSliceOfPositions(Vector3i.Axis axisOne, Vector3i.Axis axisTwo, Vector3i vector)
		=> CellDict.Keys
			.Where(
				x => x[axisOne] == vector[axisOne] && x[axisTwo] == vector[axisTwo]
				).ToArray();

	public Godot.Vector3 MapToReal(Vector3i pos)
		=> MapToReal(pos, Godot.Vector3.Zero);

	public Godot.Vector3 MapToReal(Vector3i pos, Godot.Vector3 offset)
		=> pos * CellSize + offset;

	public Vector3i[] GetIntersectableCells()
		=> (from pair in CellDict where pair.Value.IntersectEnabled select pair.Key).ToArray();

	public Aabb GetAABB(Vector3i cellPos)
	{
		Godot.Vector3 vector = MapToReal(cellPos);
		Godot.Vector3 pos = new Godot.Vector3(vector.X, vector.Y, vector.Z);
		Godot.Vector3 size = new(CellSize.X, CellSize.Y, CellSize.Z);
		return new(pos, size);
	}

	public Cell IntersectRay(Godot.Vector3 origin, Godot.Vector3 direction)
	{
		foreach (var position in GetIntersectableCells())
		{
			bool intersects = GetAABB(position).IntersectsSegment(origin, origin + direction * 99999);

			if (intersects)
				return CellGet(position);
		}
		return Cell.INVALID;
	}
}
