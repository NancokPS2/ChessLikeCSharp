using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.World.Encounter;
using Godot;
using System;

[GlobalClass]
public partial class CombatStateController : Node3D
{
    protected EBattleState StatePrevious;
    protected EBattleState StateCurrent
    {
        get => stateCurrent;
        set
        {
            StatePrevious = StateCurrent;
            stateCurrent = value;
            EventBus.BattleStateChanged?.Invoke(value);
        }

    }
    private EBattleState stateCurrent;

    public CombatStateController()
    {
        EventBus.MobTurnStarted += OnMobTurnStarted;
        EventBus.TargetingUsageParametersGenerated += OnActionUsageParametersGenerated;
        EventBus.InputBack += OnInputBack;
        EventBus.CombatStarted += OnCombatStarted;
    }

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
    }
    #endregion
}
