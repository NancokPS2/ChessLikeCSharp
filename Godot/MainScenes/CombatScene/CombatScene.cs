using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.World;
using ChessLike.World.Encounter;
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

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public static UsageParameters? UsageParameters;

	protected static EBattleState StatePrevious;
	protected static EBattleState StateCurrent
	{
		get => stateCurrent;
	}
	private static EBattleState stateCurrent;

	public CombatScene()
	{
		EventBus.TargetingUsageParametersGenerated += OnTargetingUsageParametersGenerated;
		EventBus.MobTurnStarted += OnMobTurnStarted;
		EventBus.InputBack += OnInputBack;
		EventBus.CombatStarted += OnCombatStarted;
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
	}

	public void Setup(EncounterData encounterToLoad)
	{
		EncounterData = encounterToLoad;

		EventBus.EncounterLoading?.Invoke(encounterToLoad);

		foreach (KeyValuePair<Vector3i, Mob> pair in encounterToLoad.MobPlacement)
		{
			Mob mob = pair.Value;
			if (mob is null) return;

			mob.MobState = ChessLike.Entity.EMobState.COMBAT;
			//WIP This should be used automatically
			mob.Move(new(pair.Key));
		}

		//Everything must be loaded by now.
		EventBus.CombatStarted?.Invoke();
	}

	public static EBattleState GetState() => StateCurrent;

	protected static void SetState(EBattleState state)
	{
		StatePrevious = StateCurrent;
		stateCurrent = state;
		EventBus.BattleStateChanged?.Invoke(state);
	}

	public static GridNode GetGridNode() => GridNode;

	public static Grid GetGrid() => GridNode.GetGrid();

	public static EncounterData GetEncounterData() => EncounterData;

	public static List<Mob> GetMobsInCombat() => Global.ManagerMob.GetPooledInCombat();

	#region Event Handling
	private void OnCombatStarted()
	{
		SetState(EBattleState.TURN_SELECTION);
	}

	private void OnInputBack()
	{
		switch (StateCurrent)
		{
			case EBattleState.PAUSED:
				if (StatePrevious == EBattleState.PAUSED || StatePrevious == EBattleState.INVALID)
					throw new Exception("The previous state is not valid!");

				SetState(StatePrevious);
				break;

			case EBattleState.TARGETING:
				SetState(EBattleState.ACTION_INPUT);
				break;

			default: break;
		}
	}

	private void OnMobTurnStarted(Mob mob)
	{
		SetState(EBattleState.ACTION_INPUT);
	}

	private void OnTargetingUsageParametersGenerated(UsageParameters parameters)
	{
		UsageParameters = parameters;
		SetState(EBattleState.TARGETING);
	}

	private void OnTargetingParametersDone(UsageParameters parameters)
	{
		UsageParameters = parameters;
		SetState(EBattleState.ACTION_RUNNING);
	}

	private void OnMobTurnEnded(Mob mob)
	{
		UsageParameters = null;
		SetState(EBattleState.TURN_SELECTION);
	}
	
    private void OnActionAnimationQueueEnded(List<UsageParameters> parameterList)
    {
		SetState(EBattleState.ACTION_INPUT);
    }
	#endregion

}
