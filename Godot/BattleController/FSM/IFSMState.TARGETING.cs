using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateTargeting : BattleControllerState
{
    private List<Vector3i> _last_param_positions = new();
private PopupButtonDialogUI _popup = new PopupButtonDialogUI().GetInstantiatedScene<PopupButtonDialogUI>();

    public BattleControllerStateTargeting(BattleController.State identifier) : base(identifier)
    {
    }
    
    public override void StateOnEnter()
    {
        List<Vector3i> range_to_mark = User.InputActionSelected.TargetingGetPositionsInRange(User.TurnUsageParameters);

        GD.Print(range_to_mark.ToArray());

        User.CompDisplayGrid.MeshSet(
            range_to_mark, 
            GridNode.Layer.TARGETING, 
            Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_TARGETABLE)
            );
    }

    public override void StateOnExit()
    {
        User.CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
        User.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
        
        User.PositionSelected = Vector3i.INVALID;
        _last_param_positions = new();

        if (User.StateCurrent is not BattleControllerStateActionRunning)
        {
            User.InputActionSelected = null;
            User.TurnUsageParameters.PositionsTargeted = new();
        }
        //User.CompCombatUI.NodeConfirmationUI.Update(false);
    }

    public override void StateProcess(double delta)
    {

        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.PAUSED);
        }

        //Handle AoE displaying when changing the targeted positions.
        if (User.TurnUsageParameters.PositionsTargeted.Count != 0 && User.TurnUsageParameters.PositionsTargeted != _last_param_positions)
        {
            UpdateAoEVisuals();
            _last_param_positions = User.TurnUsageParameters.PositionsTargeted;
        }

        User.UpdateCursorMovement();
        User.UpdateCameraPosition(delta);
        User.UpdateHoveredMobUI();

        //If CANCEL pressed, return to AWAITING_ACTION.
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.CANCEL))
        {
            User.FSMSetState(BattleController.State.AWAITING_ACTION);
        }
        //If ACCEPT pressed, select the position.
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.ACCEPT))
        {
            //If can still select positions, do so.
            if (HasTargetPositionsRemaining())
            {
                User.TurnUsageParameters.PositionsTargeted.Add(User.PositionHovered);
            }
        }

        if (!HasTargetPositionsRemaining())
        {
            if (_popup.GetParent() is null && _popup.IndexLastPressed == PopupButtonDialogUI.NO_INDEX)
            {
                _popup
                    .SetMessage("Confirm action?")
                    .Setup<PopupButtonDialogUI.EConfirmCancel>(User);
            }

            if (_popup.IndexLastPressed == (int)PopupButtonDialogUI.EConfirmCancel.CONFIRM)
            {
                User.CompActionRunner.Add(User.InputActionSelected, User.TurnUsageParameters);
                User.FSMSetState(BattleController.State.ACTION_RUNNING);
                _popup.Reload();
            }
            else if (_popup.IndexLastPressed == (int)PopupButtonDialogUI.EConfirmCancel.CANCEL)
            {
                User.TurnUsageParameters.PositionsTargeted.Clear();
                UpdateAoEVisuals();
                _popup.Reload();
            }
        }
    }

    private bool HasTargetPositionsRemaining() => User.TurnUsageParameters.PositionsTargeted.Count < User.InputActionSelected.TargetParams.MaxTargetedPositions;


    private void UpdateAoEVisuals()
    {
        if (User.TurnUsageParameters.PositionsTargeted.Count != 0)
        {
            List<Vector3i> aoe_marks = User.InputActionSelected.TargetingGetPositionsInAoE(User.TurnUsageParameters);

            User.CompDisplayGrid.MeshSet(
                aoe_marks, 
                GridNode.Layer.AOE, 
                Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_AOE)
                );
        }
        else
        {
            User.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
        }
    }
}
