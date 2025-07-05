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
	public static EncounterData EncounterData;

	[Export]
	private GridNode gridNode
	{
		set => GridNode = value;
		get => GridNode;
	}
	public static GridNode GridNode;
	public static Grid Grid
	{
		get => GridNode.GetGrid();
	}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public static UsageParameters? UsageParameters;

	public static Mob? MobTakingTurn;

    protected static EBattleState StatePrevious;
    protected static EBattleState StateCurrent
    {
        get => stateCurrent;
        set
        {
            StatePrevious = StateCurrent;
            stateCurrent = value;
            EventBus.BattleStateChanged?.Invoke(value);
        }

    }
    private static EBattleState stateCurrent;

	public CombatScene()
	{
		EventBus.TargetingUsageParametersGenerated += OnTargetingUsageParametersGenerated;
		EventBus.MobTurnStarted += OnMobTurnStarted;
        EventBus.InputBack += OnInputBack;
        EventBus.CombatStarted += OnCombatStarted;
	}


	public void Setup(EncounterData encounterToLoad)
	{
		EncounterData = encounterToLoad;

		EventBus.EncounterLoading?.Invoke(encounterToLoad);

		foreach (var item in encounterToLoad.MobPlacement)
		{
			if (item.PresetMob is null) return;

			item.PresetMob.MobState = ChessLike.Entity.EMobState.COMBAT;
			//WIP This should be used automatically
			item.PresetMob.Move(new(item.Location));
		}

		//Everything must be loaded by now.
		EventBus.CombatStarted?.Invoke();
	}

	public static EBattleState GetState() => StateCurrent;

	#region Event Connection
	private void OnCombatStarted()
	{
		StateCurrent = EBattleState.AWAITING_TURN;
	}
    
    private void OnInputBack()
    {
        switch (StateCurrent)
        {
            case EBattleState.PAUSED:
                if (StatePrevious == EBattleState.PAUSED || StatePrevious == EBattleState.INVALID)
                    throw new Exception("The previous state is not valid!");

                StateCurrent = StatePrevious;
                break;

            case EBattleState.TARGETING:
                StateCurrent = EBattleState.AWAITING_ACTION;
                break;

            default: throw new Exception("Unsupported state.");
        }
    }

    private void OnActionUsageParametersGenerated(UsageParameters parameters)
    {
        if (StateCurrent != EBattleState.AWAITING_ACTION) throw new Exception("How did it choose an action outside the state?");
        StateCurrent = EBattleState.TARGETING;
    }

    private void OnMobTurnStarted(Mob mob)
    {
        StateCurrent = EBattleState.AWAITING_ACTION;
		MobTakingTurn = mob;
    }

	private void OnTargetingUsageParametersGenerated(UsageParameters parameters)
	{
		UsageParameters = parameters;
		StateCurrent = EBattleState.TARGETING;
	}
	#endregion
	
}
