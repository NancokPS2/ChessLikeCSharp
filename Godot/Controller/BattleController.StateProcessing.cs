using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    private Vector3i last_position_selected;
    public enum State
    {
        PAUSED,
        AWAITING_ACTION,
        TARGETING,
    }

    public State state_current = State.PAUSED;

    public void SetState(State state)
    {
        //Exiting the state
        switch (state)
        {
            case State.TARGETING:
                display_grid.MeshRemove(GridDisplay.Layer.TARGETING);
                display_grid.MeshRemove(GridDisplay.Layer.AOE);
                break;

            case State.AWAITING_ACTION:
                break;

            default:
                break;
        }
        state_current = state;
        //Entering it
        switch (state)
        {
            case State.TARGETING:
                break;

            case State.AWAITING_ACTION:
                break;

            default:
                break;
        }
        
    }

    public void ProcessState()
    {
        switch (state_current)
        {
            case State.AWAITING_ACTION:
                ProcessAwaitingAction();
                break;

            case State.TARGETING:
                ProcessAwaitingAction();
                break;

            default:
                break;
        }
        
    }
    public void ProcessAwaitingAction()
    {
        if (action_selected != null)
        {
            //TODO: Owner cannot be null
            usage_params_in_construction = new UsageParams(null, grid, action_selected);
            SetState(State.TARGETING);
        }
    }

    public void ProcessTargeting()
    {
        bool position_changed = false;
        if (position_selected != last_position_selected)
        {
            position_changed = true;
        }

        if (position_changed)
        {
            display_grid.MeshRemove(GridDisplay.Layer.TARGETING);
            display_grid.MeshRemove(GridDisplay.Layer.AOE);

            foreach (Vector3i position in ProcessTargetingUpdateActionRange())
            {
                display_grid.MeshSet(position, GridDisplay.Layer.TARGETING, MESH_TARGETING);
            }
            foreach (Vector3i position in ProcessTargetingUpdateActionAoE())
            {
                display_grid.MeshSet(position, GridDisplay.Layer.TARGETING, MESH_AOE);
            }

        }




    }

    //TODO
    public List<Vector3i> ProcessTargetingUpdateActionRange()
    {

        List<Func<Vector3i,bool>> filters = new();


        List<Vector3i> output = new();
        output = grid.GetCellsWithinDistance(
            mob_taking_turn.Position,
            action_selected.target_params.TargetingRange,
            null);
        output = Targeter.GetSelectableCells(usage_params_in_construction);
        return output;
    }
    public List<Vector3i> ProcessTargetingUpdateActionAoE()
    {
        List<Vector3i> output = new();

        output.Add(position_selected);
        return output;
    }

}
