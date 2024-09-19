using ChessLike.Entity;
using ChessLike.Extension;
using ChessLike.Turn;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{

    public enum State
    {
        PAUSED,
        TAKING_TURN,
        ENDING_TURN,
        AWAITING_ACTION,
        TARGETING,
    }


    public void SetState(State new_state)
    {
        State state_exited = StateCurrent;
        //Exiting the state
        switch (StateCurrent)
        {
            case State.TARGETING:
                CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
                CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
                break;

            case State.AWAITING_ACTION:
                CompDisplayMob.MobUINode.EnableActionButtons(false);
                break;

            case State.PAUSED:
                NodePauseMenu.RemoveSelf();
                CompCamera.SetControl(true);
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
                CompTurnManager.StartTurn();
                CompDisplayMob.MobUINode.UpdateDelayList(CompTurnManager);
                SetState(State.AWAITING_ACTION);
                break;

            case State.ENDING_TURN:
                CompTurnManager.EndTurn();
                CompDisplayMob.MobUINode.UpdateDelayList(CompTurnManager);
                SetState(State.TAKING_TURN);
                break;

            case State.TARGETING:
                break;

            case State.AWAITING_ACTION:
                CompDisplayMob.MobUINode.EnableActionButtons(true); 
                CompDisplayMob.MobUINode.UpdateStatNodes(CompTurnManager.GetCurrentTurnTaker() as Mob);
                CompDisplayMob.MobUINode.UpdateActionButtons(CompTurnManager.GetCurrentTurnTaker() as Mob);
                break;

            case State.PAUSED:
                StatePrePause = state_exited;
                CompCamera.SetControl(false);
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
                UpdateMobUI();
                break;

            case State.TARGETING:
                UpdateCameraPosition(delta);
                ProcessTargetingState(delta);
                UpdateMobUI();
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
        if (TurnActionSelected != null)
        {
            //TODO: Owner cannot be null
            TurnUsageParameters = new UsageParams(CompTurnManager.GetCurrentTurnTaker() as Mob, CompGrid, TurnActionSelected);
            SetState(State.TARGETING);
        }
    }

    private Vector3i _last_position_selected = new Vector3i(1);
    public void ProcessTargetingState(double delta)
    {
        //Selected a new position.
        if (PositionHovered != _last_position_selected)
        {
            ProcessTargetingUpdateSelectedPositionVisuals();
            _last_position_selected = PositionHovered;
        }
    }

    public void ProcessTargetingUpdateSelectedPositionVisuals()
    {
        CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
        CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);

        //Show targetable range.
        foreach (Vector3i position in Targeter.GetSelectableCells(TurnUsageParameters))
        {
            CompDisplayGrid.MeshSet(
                position,
                GridNode.Layer.TARGETING, 
                MESH_TARGETING
                );
        }

        //Show AoE
        foreach (Vector3i position in Targeter.GetTargetedAoE(PositionHovered, TurnUsageParameters))
        {
            CompDisplayGrid.MeshSet(
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
        if (CompDisplayGrid.InputEnable)
        {
            if (!CompGrid.IsPositionInbounds(CompDisplayGrid.PositionHovered)){return;}

            PositionHovered = CompDisplayGrid.PositionHovered;
            CompDisplayGrid.MeshRemove(GridNode.Layer.CURSOR);
            CompDisplayGrid.MeshSet(PositionHovered, GridNode.Layer.CURSOR, MESH_CURSOR);
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
            if ( CompGrid.IsPositionInbounds( move + PositionHovered ))
            {
                CompDisplayGrid.MeshRemove(GridNode.Layer.CURSOR);
                PositionHovered += move;
                CompDisplayGrid.MeshSet(PositionHovered, GridNode.Layer.CURSOR, MESH_CURSOR);
                GD.Print(move.ToString());
            }else
            {
                GD.PushError(string.Format("Position {0} out of bounds.", (move + PositionHovered).ToString()));
            }
        }

    }

    public void UpdateCameraPosition(double delta)
    {
        Vector3 camera_pivot = CompCamera.pivot_point;

        if (camera_pivot.DistanceTo(PositionHovered.ToGVector3()) > 3)
        {
            camera_pivot = camera_pivot.Lerp(PositionHovered.ToGVector3(), (float)delta);
        }
    }

    public void UpdateMobUI()
    {
        //TODO
        List<Mob>? mob_list = Global.ManagerMob.GetInPosition(PositionHovered);
        if (mob_list.Count == 0) {return;}
        
        Mob mob = mob_list.First();

        if (mob is not null && CompDisplayMob.MobUINode.GetOwnerOfStats() != mob)
        {
            CompDisplayMob.MobUINode.UpdateStatNodes(mob);
        }
        //display_mob.MobUINode.UpdateStatNodes();
    }
}
