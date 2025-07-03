using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public partial class BattleController
{

    public List<BattleControllerState> StateList { get; set; } = new()
    {
        new BattleControllerStatePaused(EBattleState.PAUSED),
        new BattleControllerStateTakingTurn(EBattleState.TAKING_TURN),
        new BattleControllerStateEndingTurn(EBattleState.ENDING_TURN),
        new BattleControllerStateTargeting(EBattleState.TARGETING),
        new BattleControllerStateAwaitingAction(EBattleState.AWAITING_ACTION),
        new BattleControllerStateActionRunning(EBattleState.ACTION_RUNNING),
        new BattleControllerStatePreparation(EBattleState.PREPARATION),
        new BattleControllerStateEndCombat(EBattleState.END_COMBAT),
    };

    private BattleControllerState _queued_state;
    public float ProcessDelta { get; set; }
    public BattleControllerState StateCurrent { get; set; }

    public void FSMSetup()
    {
        foreach (var item in StateList)
        {
            item.User = this;
        }
        FSMSetState(EBattleState.PREPARATION);
    }

    public void FSMSetState(BattleControllerState state)
    {
        _queued_state = state;
    }
    public void FSMSetState(EBattleState state)
    {
        FSMSetState(StateList.First(x => x.StateIdentifier == state));
    }

    public BattleControllerState StatePrevious;
    public void FSMProcess(double delta)
    {
        if (StateCurrent != _queued_state)
        {
            StatePrevious = StateCurrent;
            StateCurrent = _queued_state;
            if (!StateList.Contains(StateCurrent)) {throw new Exception("This state is not in the list.");}

            if (StatePrevious is BattleControllerState not_null)
            {
                not_null.StateOnExit();
            }
            StateCurrent.StateOnEnter();

            //Emit the state change
            if (StatePrevious is not null && StateCurrent is not null)
                EventBus.BattleStateChanged?.Invoke(StateCurrent.StateIdentifier);

            StateTimeWithoutChange = 0;
        }

        StateTimeWithoutChange += (float)delta;

        StateCurrent.StateProcess(delta);
    }
}
