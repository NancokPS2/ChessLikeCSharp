using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Extension;
using ChessLike.Storage;
using ChessLike.World;
using ChessLike.WorldMap;
using Godot;

[GlobalClass]
public partial class CombatScene : Node3D
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	protected static EncounterData EncounterData;

	[Export]
	private GridNode gridNode
	{
		set => GridNode = value;
		get => GridNode;
	}
	protected static GridNode GridNode;

	[Export]
	private MobMovement mobMovement
	{
		set => MobMovementNode = value;
		get => MobMovementNode;
	}
	protected static MobMovement MobMovementNode;

	protected static Grid GridResource;

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public static UsageParameters? UsageParameters;

	protected static ECombatState StatePrevious;
	protected static ECombatState StateCurrent
	{
		get => stateCurrent;
	}
	private static ECombatState stateCurrent;


	public override void _Ready()
	{
		EventBus.CombatPreparationStarted += OnCombatPreparationStarted;
		EventBus.CombatPreparationEnded += OnCombatPreparationEnded;
		EventBus.CombatStarted += OnCombatStarted;
		EventBus.CombatEnded += OnCombatEnded;
		EventBus.TargetingUsageParametersGenerated += OnTargetingUsageParametersGenerated;
		EventBus.MobTurnStarted += OnMobTurnStarted;
		EventBus.InputBack += OnInputBack;
		EventBus.InputPause += OnInputPause;
		EventBus.TargetingParametersDone += OnTargetingParametersDone;
		EventBus.MobTurnEnded += OnMobTurnEnded;
		EventBus.ActionAnimationQueueEnded += OnActionAnimationQueueEnded;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if (@event.IsActionPressed("cancel"))
		{
			EventBus.InputBack?.Invoke();
		}
		else if (@event.IsActionPressed("pause"))
		{
			EventBus.InputPause?.Invoke();
		}
	}

	public void Setup(EncounterData encounterToLoad)
	{
		EncounterData = encounterToLoad;
		GridResource = EncounterData.Grid;
		EventBus.GridChanged?.Invoke(EncounterData.Grid);

		//Create mobs
		List<EFaction> factionsPresent = new();
		List<Mob> mobsToAdd = new();
		foreach (MobSpawn spawn in encounterToLoad.MobSpawns)
		{
			//Generate the Mob
			Mob mob;

			if (spawn.Mob is not null)
			{
				mob = spawn.Mob;
			}
			else
			{
				mob = spawn.GetNewMob();
			}

			mob.MobState = ChessLike.Entity.EMobState.COMBAT;
			mobsToAdd.Add(mob);
			if (!factionsPresent.Contains(mob.Faction))
				factionsPresent.Add(mob.Faction);
		}

		//Setup the GridRandom if present.
		//WIP (should not force all GridRandoms to allow the PLAYER faction)
		if (!factionsPresent.Contains(EFaction.PLAYER))
			factionsPresent.Add(EFaction.PLAYER);
		SetupGridRandom(factionsPresent);

		//Find all spawn points
		List<(Vector3i, EFaction)> spawnPoints = (
			from pair
			in GetGrid().CellDictionary
			where pair.Value.FactionSpawn != EFaction.INVALID
			select (pair.Key, pair.Value.FactionSpawn)).ToList();

		//Find where to place the mobs from MobSpawns
		foreach (var mob in mobsToAdd)
		{
			Vector3i pos = spawnPoints
				.Where(x => x.Item2 == mob.Faction)
				.ToList()
				.PopRandom()
				.Item1;

			//Make sure the mob can be there.
			if (!MobMovementNode.IsPositionValidToExist(mob, pos))
				throw new Exception($"Mob {mob.DisplayedName} has been spawned in an invalid location {pos}");

			//WIP This should be used automatically
			GetMobMovement().ForceMobPosition(mob, pos);;
		}

		EventBus.EncounterLoaded?.Invoke(encounterToLoad);
		EventBus.GridChanged?.Invoke(GetGrid());
		//Everything must be loaded by now.
		EventBus.CombatPreparationStarted?.Invoke();
	}

	private static void SetupGridRandom(List<EFaction> factionsPresent)
	{
		if (GetGrid() is GridRandom rand)
		{
			//Update the map to support all the factions that will be in it.
			rand.SetFactionsSupported(factionsPresent);
			rand.Randomize();
		}
	}


	public static ECombatState GetState() => StateCurrent;

	protected static void SetState(ECombatState state)
	{
		StatePrevious = StateCurrent;
		stateCurrent = state;
		EventBus.CombatStateChanged?.Invoke(state);
	}

	public static GridNode GetGridNode() => GridNode;

	public static Grid GetGrid() => GridResource;

	public static EncounterData GetEncounterData() => EncounterData;

	public static List<Mob> GetMobsInCombat() => Global.ManagerMob.GetPooledInCombat();

	public static MobMovement GetMobMovement() => MobMovementNode;

	#region Event Handling
	private void OnCombatPreparationStarted()
	{
		SetState(ECombatState.PREPARATION);
	}

	private void OnCombatPreparationEnded()
	{
		EventBus.CombatStarted?.Invoke();
	}

	private void OnCombatStarted()
	{
		SetState(ECombatState.TURN_SELECTION);
	}

	private void OnCombatEnded()
	{
		SetState(ECombatState.END_COMBAT);
	}

	private void OnInputBack()
	{
		switch (StateCurrent)
		{
			case ECombatState.PAUSED:
				if (StatePrevious == ECombatState.PAUSED && StatePrevious == ECombatState.INVALID)
					throw new Exception("The previous state is not valid!");

				SetState(StatePrevious);
				break;

			case ECombatState.TARGETING:
				SetState(ECombatState.ACTION_INPUT);
				break;

			default: break;
		}
	}

	private void OnInputPause()
	{
		if (StateCurrent == ECombatState.PAUSED)
		{
			SetState(StatePrevious);
		}
		else
		{
			SetState(ECombatState.PAUSED);
		}
	}

	private void OnMobTurnStarted(Mob mob)
	{
		SetState(ECombatState.ACTION_INPUT);
	}

	private void OnTargetingUsageParametersGenerated(UsageParameters parameters)
	{
		UsageParameters = parameters;
		SetState(ECombatState.TARGETING);
	}

	private void OnTargetingParametersDone(UsageParameters parameters)
	{
		UsageParameters = parameters;
		SetState(ECombatState.ACTION_RUNNING);
	}

	private void OnMobTurnEnded(Mob mob)
	{
		UsageParameters = null;
		SetState(ECombatState.TURN_SELECTION);
	}

	private void OnActionAnimationQueueEnded(List<UsageParameters> parameterList)
	{
		SetState(ECombatState.ACTION_INPUT);
	}
	#endregion

}
