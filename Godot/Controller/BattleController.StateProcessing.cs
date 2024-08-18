using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController : IGInput
{
    const float MINIMUM_MOVEMENT_DELTA = 0.15f;
    private float delta_since_last_movement;
    private Vector3i last_position_selected = new Vector3i(1);
    public Control pause_menu_node = GD.Load<PackedScene>("res://assets/PackedScene/Pause.tscn").Instantiate<Control>();

    public enum State
    {
        PAUSED,
        AWAITING_ACTION,
        TARGETING,
    }

    public State state_current = State.AWAITING_ACTION;

    public Dictionary<IGInput.Button, bool> ButtonsEnabled { get; set; } = new();

    public void SetState(State state)
    {
        //Exiting the state
        switch (state_current)
        {
            case State.TARGETING:
                display_grid.MeshRemove(GridDisplay.Layer.TARGETING);
                display_grid.MeshRemove(GridDisplay.Layer.AOE);
                break;

            case State.AWAITING_ACTION:
                break;

            case State.PAUSED:
                GetTree().CurrentScene.RemoveChild(pause_menu_node);
                break;

            default:
                break;
        }

        state_current = state;
        
        //Entering it
        switch (state_current)
        {
            case State.TARGETING:
                break;

            case State.AWAITING_ACTION:
                break;

            case State.PAUSED:
                GetTree().CurrentScene.AddChild(pause_menu_node);
                break;

            default:
                break;
        }
        
    }

    public void ProcessState(double delta)
    {
        delta_since_last_movement += (float)delta;

        switch (state_current)
        {
            case State.AWAITING_ACTION:
                ProcessCursorMovement();
                ProcessAwaitingAction(delta);
                break;

            case State.TARGETING:
                ProcessCursorMovement();
                ProcessTargeting(delta);
                break;

            default:
                break;
        }
        
    }
    public void ProcessAwaitingAction(double delta)
    {
        if (action_selected != null)
        {
            //TODO: Owner cannot be null
            usage_params_in_construction = new UsageParams(mob_taking_turn, grid, action_selected);
            SetState(State.TARGETING);
        }
    }

    public void ProcessTargeting(double delta)
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
        display_grid.MeshRemove(GridDisplay.Layer.TARGETING);
        display_grid.MeshRemove(GridDisplay.Layer.AOE);

        //Show targetable range.
        foreach (Vector3i position in Targeter.GetSelectableCells(usage_params_in_construction))
        {
            display_grid.MeshSet(
                position,
                GridDisplay.Layer.TARGETING, 
                MESH_TARGETING
                );
        }

        //Show AoE
        foreach (Vector3i position in Targeter.GetTargetedAoE(position_selected, usage_params_in_construction))
        {
            display_grid.MeshSet(
                position, 
                GridDisplay.Layer.TARGETING, 
                MESH_AOE
                );
        }
    }

    public void ProcessCursorMovement()
    {
        //delta must be high enough to continue
        if (!(delta_since_last_movement > MINIMUM_MOVEMENT_DELTA)){return;}
        
        delta_since_last_movement = 0;
        Vector3i move = new Vector3i(((IGInput)this).InputGetMovementVector(true));
        //Stop if there was no movement.
        if (move == Vector3i.ZERO){return;}

        //Ensure that it is valid before attempting the move.
        if ( grid.IsPositionInbounds( move + position_selected ))
        {
            display_grid.MeshRemove(GridDisplay.Layer.CURSOR);
            position_selected += move;
            display_grid.MeshSet(position_selected, GridDisplay.Layer.CURSOR, MESH_CURSOR);
            GD.Print(move.ToString());
        }else
        {
            GD.PushError(string.Format("Position {0} out of bounds.", (move + position_selected).ToString()));
        }
    }
}
