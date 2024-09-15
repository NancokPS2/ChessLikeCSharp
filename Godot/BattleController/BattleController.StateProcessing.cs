using ChessLike.Entity;
using ChessLike.Extension;
using Godot.Menu;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    private State? StatePrePause;

    public Pause NodePauseMenu = new();

    public enum State
    {
        PAUSED,
        TAKING_TURN,
        ENDING_TURN,
        AWAITING_ACTION,
        TARGETING,
    }

    public State StateCurrent = State.TAKING_TURN;

    public void SetState(State new_state)
    {
        State state_exited = StateCurrent;
        //Exiting the state
        switch (StateCurrent)
        {
            case State.TARGETING:
                display_grid.MeshRemove(GridNode.Layer.TARGETING);
                display_grid.MeshRemove(GridNode.Layer.AOE);
                break;

            case State.AWAITING_ACTION:
                display_mob.MobUINode.EnableActionButtons(false);
                break;

            case State.PAUSED:
                NodePauseMenu.RemoveSelf();
                display_camera.SetControl(true);
                break;

            default:
                break;
        }

        StateCurrent = new_state;
        StateTimeWithoutChange = 0;

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
                display_mob.MobUINode.EnableActionButtons(true); 
                break;

            case State.PAUSED:
                StatePrePause = state_exited;
                display_camera.SetControl(false);
                NodePauseMenu.AddSceneWithDeclarations(Pause.SCENE_PATH, Pause.NodesRequired);
                break;

            default:
                break;
        }
        
    }

    public void ProcessState(double delta)
    {
        StateTimeWithoutChange += (float)delta;

        switch (StateCurrent)
        {
            case State.AWAITING_ACTION:
                ProcessAwaitingActionState(delta);
                UpdateCameraPosition(delta);
                break;

            case State.TARGETING:
                UpdateCameraPosition(delta);
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
        switch (StateCurrent)
        {
            case State.AWAITING_ACTION:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    SetState(State.PAUSED);
                }
                UpdateCursorMovement();
                break;

            case State.TARGETING:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    SetState(State.PAUSED);
                }
                UpdateCursorMovement();
                break;

            case State.PAUSED:
                if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
                {
                    if (StatePrePause is State not_null)
                    {
                        SetState(not_null);
                    }
                    else
                    {
                        throw new Exception("Entered pause without setting a previous state to return to!");
                    }
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
        if (PositionHovered != last_position_selected)
        {
            ProcessTargetingUpdateSelectedPositionVisuals();
            last_position_selected = PositionHovered;
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
        foreach (Vector3i position in Targeter.GetTargetedAoE(PositionHovered, UsageParameters))
        {
            display_grid.MeshSet(
                position, 
                GridNode.Layer.TARGETING, 
                MESH_AOE
                );
        }
    }

    const float MINIMUM_MOVEMENT_DELTA = 0.12f;
    private float _last_movement_time;
    public void UpdateCursorMovement()
    {
        //Decide between mouse based input and key input.
        if (display_grid.InputEnable)
        {
            if (!grid.IsPositionInbounds(display_grid.PositionHovered)){return;}

            PositionHovered = display_grid.PositionHovered;
            display_grid.MeshRemove(GridNode.Layer.CURSOR);
            display_grid.MeshSet(PositionHovered, GridNode.Layer.CURSOR, MESH_CURSOR);
        }
        else
        {
            //delta must be high enough to continue
            if (Time.GetTicksMsec() - _last_movement_time < MINIMUM_MOVEMENT_DELTA){return;}

            _last_movement_time = Time.GetTicksMsec();
            Vector3i move = new Vector3i(Global.GInput.GetMovementVector(true));

            //Stop if there was no movement.
            if (move == Vector3i.ZERO){return;}

            //Ensure that it is valid before attempting the move.
            if ( grid.IsPositionInbounds( move + PositionHovered ))
            {
                display_grid.MeshRemove(GridNode.Layer.CURSOR);
                PositionHovered += move;
                display_grid.MeshSet(PositionHovered, GridNode.Layer.CURSOR, MESH_CURSOR);
                GD.Print(move.ToString());
            }else
            {
                GD.PushError(string.Format("Position {0} out of bounds.", (move + PositionHovered).ToString()));
            }
        }

    }

    public void UpdateCameraPosition(double delta)
    {
        Vector3 camera_pivot = display_camera.pivot_point;

        if (camera_pivot.DistanceTo(PositionHovered.ToGVector3()) > 3)
        {
            camera_pivot = camera_pivot.Lerp(PositionHovered.ToGVector3(), (float)delta);
        }
    }

    public void UpdateMobUI()
    {
        //TODO
        Global.ManagerMob.GetInPosition(PositionHovered);
        //display_mob.MobUINode.UpdateStatNodes();
    }
}
