using ChessLike.Entity;
using Godot.Menu;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    private State pre_pause_state;

    public Pause pause_menu = new();

    public enum State
    {
        PAUSED,
        TAKING_TURN,
        ENDING_TURN,
        AWAITING_ACTION,
        TARGETING,
    }

    public State state_current = State.TAKING_TURN;

    public void SetState(State new_state)
    {
        State state_exited = state_current;
        //Exiting the state
        switch (state_current)
        {
            case State.TARGETING:
                display_grid.MeshRemove(GridNode.Layer.TARGETING);
                display_grid.MeshRemove(GridNode.Layer.AOE);
                break;

            case State.AWAITING_ACTION:
                display_mob.mob_ui.EnableActionButtons(false);
                break;

            case State.PAUSED:
                IGSceneAdapter.RemoveScene(pause_menu);
                display_camera.SetControl(true);
                break;

            default:
                break;
        }

        state_current = new_state;
        time_passed_this_state = 0;

        //Entering it
        switch (new_state)
        {
            case State.TAKING_TURN:
                List<ITurn> turn_takers = new List<ITurn>(Global.ManagerMob.GetInCombat());
                mob_taking_turn = (Mob)TurnQueue.GetNext(turn_takers);
                SetState(State.AWAITING_ACTION);
                break;

            case State.ENDING_TURN:
                List<ITurn> time_pass_targets = new List<ITurn>(Global.ManagerMob.GetInCombat());
                TurnQueue.AdvanceDelay(time_pass_targets, delay_this_turn);
                SetState(State.TAKING_TURN);
                break;

            case State.TARGETING:
                break;

            case State.AWAITING_ACTION:
                display_mob.mob_ui.EnableActionButtons(true); 
                break;

            case State.PAUSED:
                pre_pause_state = state_exited;
                display_camera.SetControl(false);
                IGSceneAdapter.Setup(pause_menu, this);
                break;

            default:
                break;
        }
        
    }

    public void ProcessState(double delta)
    {
        delta_since_last_movement += (float)delta;
        time_passed_this_state += (float)delta;

        switch (state_current)
        {
            case State.AWAITING_ACTION:
                ProcessAwaitingActionState(delta);
                break;

            case State.TARGETING:
                ProcessTargetingState(delta);
                break;

            case State.PAUSED:
                break;

            default:
                break;
        }
        
    }
    public void ProcessStateInput()
    {
        switch (state_current)
        {
            case State.AWAITING_ACTION:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    SetState(State.PAUSED);
                }
                ProcessCursorMovement();
                break;

            case State.TARGETING:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    SetState(State.PAUSED);
                }
                ProcessCursorMovement();
                
                break;

            case State.PAUSED:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    SetState(pre_pause_state);
                }
                break;

            default:
                break;
        }
        
    }
    public void ProcessAwaitingActionState(double delta)
    {
        if (action_selected != null)
        {
            //TODO: Owner cannot be null
            UsageParameters = new UsageParams(mob_taking_turn, grid, action_selected);
            SetState(State.TARGETING);
        }
    }

    private Vector3i last_position_selected = new Vector3i(1);
    public void ProcessTargetingState(double delta)
    {
        //Selected a new position.
        if (position_selected != last_position_selected)
        {
            ProcessTargetingUpdateSelectedPositionVisuals();
            last_position_selected = position_selected;
        }
    }

    public void ProcessTargetingUpdateSelectedPositionVisuals()
    {
        display_grid.MeshRemove(GridNode.Layer.TARGETING);
        display_grid.MeshRemove(GridNode.Layer.AOE);

        //Show targetable range.
        foreach (Vector3i position in Targeter.GetSelectableCells(UsageParameters))
        {
            display_grid.MeshSet(
                position,
                GridNode.Layer.TARGETING, 
                MESH_TARGETING
                );
        }

        //Show AoE
        foreach (Vector3i position in Targeter.GetTargetedAoE(position_selected, UsageParameters))
        {
            display_grid.MeshSet(
                position, 
                GridNode.Layer.TARGETING, 
                MESH_AOE
                );
        }
    }

    const float MINIMUM_MOVEMENT_DELTA = 0.15f;
    private float delta_since_last_movement;
    public void ProcessCursorMovement()
    {
        //delta must be high enough to continue
        if (!(delta_since_last_movement > MINIMUM_MOVEMENT_DELTA)){return;}
        
        delta_since_last_movement = 0;
        Vector3i move = new Vector3i(Global.GInput.GetMovementVector(true));

        //Stop if there was no movement.
        if (move == Vector3i.ZERO){return;}

        //Ensure that it is valid before attempting the move.
        if ( grid.IsPositionInbounds( move + position_selected ))
        {
            display_grid.MeshRemove(GridNode.Layer.CURSOR);
            position_selected += move;
            display_grid.MeshSet(position_selected, GridNode.Layer.CURSOR, MESH_CURSOR);
            display_camera.pivot_point = position_selected.ToGVector3();
            GD.Print(move.ToString());
        }else
        {
            GD.PushError(string.Format("Position {0} out of bounds.", (move + position_selected).ToString()));
        }
    }
}
