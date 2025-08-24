using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using Godot;
using Sprache;

namespace ChessLike.Entity;

public partial class MobAStar : AStar3D
{
	protected BiDictionary<Vector3i, long> PositionPointCacheDict = new();
	public override float _ComputeCost(long fromId, long toId)
	{
		var fromPos = GetPointPositionCached(fromId);
		var toPos = GetPointPositionCached(toId);
		return Mathf.Abs(fromPos.X - toPos.X) + Mathf.Abs(fromPos.Y - toPos.Y) + Mathf.Abs(fromPos.Z - toPos.Z);
	}

	public override float _EstimateCost(long fromId, long endId)
	{
		return _ComputeCost(fromId, endId);
	}

	public void CachePoints()
	{
		PositionPointCacheDict.Clear();
		foreach (var id in GetPointIds())
		{
			Vector3i position = new(GetPointPosition(id));
			PositionPointCacheDict[id] = position;
		}
		if (!CacheVerify()) throw new Exception("Caching failed.");
	}

	public bool CacheVerify()
		=> PositionPointCacheDict.Count() == GetPointIds().Count();

	public List<Vector3i> GetPath(Vector3i start, Vector3i end)
	{
		long first = GetPointIdCached(start);
		long last = GetPointIdCached(end);
		return (from pos in GetPointPath(first, last) select new Vector3i(pos)).ToList();
	}

	public long GetPointIdCached(Vector3i position)
		=> PositionPointCacheDict[position];

	public long GetPointIdCached(Godot.Vector3 position)
		=> PositionPointCacheDict[new Vector3i(position)];

	public Vector3i GetPointPositionCached(long id)
		=> PositionPointCacheDict[id];

	public Vector3i[] GetPointPositionsCached()
		=> PositionPointCacheDict.Keys.ToArray();


	public Dictionary<long, Vector3i> GetIdToPointDict()
		=> PositionPointCacheDict.GetDictReverse();

	protected Func<Vector3i, Vector3i, bool> GetConnectionCondition(Mob mobUsed, EMovementMode mode)
	=> mode switch
	{
		EMovementMode.TELEPORT => (point, pointTarget) => true,
		_ => (point, pointTarget) => point.DistanceManhattanWithToleranceTo(
			pointTarget,
			new(0, (int)mobUsed.Stats.GetStat(EStatName.JUMP), 0)) < 1,
	};
}
