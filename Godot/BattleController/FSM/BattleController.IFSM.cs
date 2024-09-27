using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public partial class BattleController
{
    public enum State
    {
        INVALID,
        PAUSED,
        TAKING_TURN,
        ENDING_TURN,
        AWAITING_ACTION,
        TARGETING,
        ACTION_RUNNING,
    }

    public List<BattleControllerState> StateList { get; set; } = new()
    {
        new BattleControllerStatePaused(State.PAUSED),
        new BattleControllerStateTakingTurn(State.TAKING_TURN),
        new BattleControllerStateEndingTurn(State.ENDING_TURN),
        new BattleControllerStateTargeting(State.TARGETING),
        new BattleControllerStateAwaitingAction(State.AWAITING_ACTION),
        new BattleControllerStateActionRunning(State.ACTION_RUNNING),
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
        FSMSetState(State.TAKING_TURN);
    }

    public void FSMSetState(BattleControllerState state)
    {
        _queued_state = state;
    }
    public void FSMSetState(State state)
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
            StateTimeWithoutChange = 0;
        }

        StateTimeWithoutChange += (float)delta;

        StateCurrent.StateProcess(delta);
    }
}
