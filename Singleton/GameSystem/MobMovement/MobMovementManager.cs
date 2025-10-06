using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using ChessLike.Context;
using ChessLike.Extension;
using ChessLike.World;
using ChessLike.WorldMap;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.Entity;
//public Navigation navigation = new();

[GlobalClass]
public partial class MobMovementManager : Node3D, ISingleton<MobMovementManager>
{
	public static MobMovementManager Instance { get; set; } = null!;
	public static Grid GridUsed = null!;

	public static Dictionary<(Mob, EMovementMode), MobAStar> AStars = new();

	public override void _Ready()
	{
		base._Ready();
		EventBus.MobStateChanged += OnMobStateChanged;
		EventBus.CombatGridChanged += OnGridChanged;
		EventBus.MobMovementPathRequested += OnMobMovementRequested;
		EventBus.MobCellListChanged += OnMobCellListChanged;

		Instance = this;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		EventBus.MobStateChanged -= OnMobStateChanged;
		EventBus.CombatGridChanged -= OnGridChanged;
		EventBus.MobMovementPathRequested -= OnMobMovementRequested;
		EventBus.MobCellListChanged -= OnMobCellListChanged;
	}


	public static List<Vector3i> GetPathablePositions(Mob mob, EMovementMode moveMode)
	{
		ErrorOnMissingMove(mob, moveMode);
		var aStar = GetAStar(mob, moveMode);

		List<Vector3i> output = new();
		foreach (var pointPos in aStar.GetPointPositionsCached())
		{
			if (IsPositionReachable(mob, mob.GetPosition(), pointPos, moveMode))
				output.Add(pointPos);
		}

		return output;
	}

	private static bool IsPositionReachable(Mob mob, Vector3i from, Vector3i target, EMovementMode moveMode)
	{
		MobAStar aStar = GetAStar(mob, moveMode);
		List<Vector3i> path = aStar.GetPath(from, target);

		int maxRange = GetHorizontalRange(mob);
		int pathCount = path.Count;

		bool endsInTarget = path.Last() == target;
		bool correctPointCount = pathCount != 0 && pathCount <= maxRange;

		//If failed
		return correctPointCount && endsInTarget;
	}


	public static MobAStar GetAStar(Mob mob, EMovementMode moveMode, bool createIfMissing = false)
	{
		(Mob, EMovementMode) key = (mob, moveMode);
		if (AStars.ContainsKey(key))
		{
			return AStars[key];
		}
		else if (createIfMissing)
		{
			AStars[key] = GetNewAStar(mob, moveMode);
			ValidateAStarCache(mob, moveMode);
			return GetAStar(mob, moveMode, false);
		}
		else
		{
			throw new Exception($"Missing {typeof(MobAStar).ToString().GetExtension()} for {mob.DisplayedName}");
		}
	}

	protected static MobAStar GetNewAStar(Mob mob, EMovementMode moveMode)
	{
		if (GridUsed is null) throw new Exception();

		MobAStar aStar = new();
		//Reserve space first to speed it up.

		aStar.ReserveSpace(
			GridUsed.Boundary.X * GridUsed.Boundary.Y * GridUsed.Boundary.Z
			);

		//Add the points

		foreach (var item in GridUsed.CellDictionary)
		{
			if (IsPositionValidToExist(mob, item.Key))
				aStar.AddPoint(aStar.GetAvailablePointId(), item.Key.ToGVector3());
		}
		aStar.CachePoints();

		//Connect the points

		foreach (var pointId in aStar.GetPointIds())
		{
			Vector3i point = aStar.GetPointPositionCached(pointId);
			foreach (var pointTargetId in aStar.GetPointIds())
			{
				Vector3i pointTarget = new(aStar.GetPointPositionCached(pointTargetId));

				//If it can move there, connect it.

				if (IsValidMove(mob, point, pointTarget, moveMode))
					aStar.ConnectPoints(pointId, pointTargetId);
			}
		}
		return aStar;
	}

	public static void ValidateAStarCache()
	{
		foreach (var item in AStars)
		{
			ValidateAStarCache(item.Key.Item1, item.Key.Item2);
		}
	}
	public static void ValidateAStarCache(Mob mob, EMovementMode moveMode)
	{
		foreach (var item in GetAStar(mob, moveMode).GetPointPositionsCached())
		{
			if (!IsPositionValidToExist(mob, item))
				throw new Exception($"Invalid cache. {item} is not a valid position to be in for {mob.DisplayedName}.");
		}
	}

	protected static bool IsValidMove(Mob mob, Vector3i from, Vector3i to, EMovementMode mode)
	{
		bool canStandAtTarget = IsPositionValidToExist(mob, to);

		bool reachable = mode switch
		{
			EMovementMode.PLACE => true,

			EMovementMode.TELEPORT =>
				from.DistanceManhattanWithToleranceTo(
					to,
					new(0, int.MaxValue, 0)
					) <= GetHorizontalRange(mob),

			_ =>
				from.DistanceManhattanWithToleranceTo(
						to,
						new(0, GetVerticalRange(mob), 0)
					) == 1,
		};

		return reachable && canStandAtTarget;
	}

	public static bool IsPositionValidToExist(Mob mob, Vector3i position)
	{
		//Cannot step on the bottom of the world!!!
		if (position.Y < 1) return false;

		bool canExist =
			mob.CellExistWhitelist.All(x => GridUsed.IsFlagInPosition(position, x))
			&& mob.CellExistBlacklist.All(x => !GridUsed.IsFlagInPosition(position, x));
		bool canStandOn =
			mob.CellStandWhitelist.All(x => GridUsed.IsFlagInPosition(position + Vector3i.DOWN, x))
			&& mob.CellStandBlacklist.All(x => !GridUsed.IsFlagInPosition(position + Vector3i.DOWN, x));
		return canExist && canStandOn;
	}

	public bool IsPositionValidToSpawn(Vector3i position, Mob mob)
	{
		if (mob is null) throw new Exception();

		//The position must be valid for this mob.
		if (!IsPositionValidToExist(mob, position))
			return false;

		//Must be a valid spot to place mobs
		if (GridUsed.GetCell(position).FactionSpawn != mob.Faction)
			return false;

		return true;
	}

	public static int GetHorizontalRange(Mob mob)
	{
		return (int)mob.Stats.GetStat(EStatName.MOVEMENT);
	}

	public static int GetVerticalRange(Mob mob)
	{
		return (int)mob.Stats.GetStat(EStatName.JUMP);
	}

	/// <summary>
	/// Forcibly sets the position of a mob, ignoring any sort of pathfinding.
	/// It does this by invoking a MobMoved event.
	/// </summary>
	/// <param name="mob">What to move.</param>
	/// <param name="where">Where to move it.</param>
	public void ForceMobPosition(Mob mob, Vector3i where)
	{
		mob.SetPosition(where);
		EventBus.MobMoved?.Invoke(new MobMovementContext(mob, EMovementMode.PLACE, new(){where}));
	}

	private void UpdateAStar()
	{
		foreach (var item in AStars)
		{
			UpdateAStar(item.Key.Item1);
		}
	}

	private void UpdateAStar(Mob mob, List<EMovementMode>? ignored = null)
	{
		ignored ??= new();
		foreach (var moveMode in mob.GetMovementModesFromAbilities())
		{
			if (ignored.Contains(moveMode)) continue;

			MobAStar newAStar = GetNewAStar(mob, moveMode);
			AStars[(mob, moveMode)] = newAStar;
			ValidateAStarCache(mob, moveMode);
		}
	}

	public static void ErrorOnMissingMove(Mob mob, EMovementMode moveMode)
	{
		if (!mob.GetMovementModesFromAbilities().Contains(moveMode))
			MsgLog.Log(EMessageType.ERROR, $"Making a MobAStar with mode {moveMode} for {mob.DisplayedName}. Which does not have that mode.");
	}

	#region Event Handling
	private void OnGridChanged(Grid obj)
	{
		GridUsed = obj;
		UpdateAStar();
	}

	private void OnMobStateChanged(Mob mob, EMobState state)
	{
		if (state != EMobState.COMBAT) return;
		UpdateAStar(mob);
	}

	private void OnMobMovementRequested(MobMovementContext context)
	{
		var path = context.Path;
		var mob = context.Mover;
		EMovementMode moveMode = context.MovementMode;
		if (IsPositionReachable(mob, mob.GetPosition(), path.Last(), moveMode))
		{
			foreach (var pos in path)
			{
				mob.SetPosition(pos);
			}
			EventBus.MobMoved?.Invoke(context);
		}
		else
		{
			MsgLog.Log(EMessageType.ERROR, $"Issued a path movement for {mob.DisplayedName} that is not valid: {path.ToStringList(", ")}.");
		}
	}

	private void OnMobCellListChanged(Mob obj)
	{
		UpdateAStar(obj);
	}
	#endregion
}
