using ChessLike.Entity;
using ChessLike.Entity.Action;
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
        EventBus.InputActionSelected += OnInputActionSelected;
        EventBus.InputBack += OnInputBack;
    }



    #region Event Connection
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

    [Obsolete("Something should confirm the action to THEN trigger this, no?")]
    private void OnInputActionSelected(Ability action)
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
