using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using ExtendedXmlSerializer;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateTargeting : BattleControllerState
{
    private List<Vector3i> _last_param_positions = new();
    private PopupButtonDialogUI _popup = new PopupButtonDialogUI().GetInstantiatedScene<PopupButtonDialogUI>();
    private List<Vector3i> _pos_valid_for_targeting = new();

    public BattleControllerStateTargeting(BattleController.State identifier) : base(identifier)
    {
    }


    public override void StateOnEnter()
    {
        _pos_valid_for_targeting = User.InputActionSelected.TargetParams.GetTargetedPositions(User.TurnUsageParameters);

        BattleController.CompDisplayGrid.MeshSet(
            _pos_valid_for_targeting, 
            GridNode.Layer.TARGETING, 
            Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_TARGETABLE)
            );
    }

    public override void StateOnExit()
    {
        BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
        BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
        
        User.PositionSelected = Vector3i.INVALID;
        _last_param_positions = new();

        //Is it about to run the action? If not, also reset these values.
        if (User.StateCurrent is not BattleControllerStateActionRunning && User.StateCurrent is not BattleControllerStatePaused)
        {
            User.InputActionSelected = null;
            User.TurnUsageParameters.PositionsTargeted = new();
        }
        _pos_valid_for_targeting = new();
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
            //If can still select positions, add it to the usage parameters.
            if (HasTargetPositionsRemaining() && _pos_valid_for_targeting.Contains(User.PositionHovered))
            {
                User.TurnUsageParameters.PositionsTargeted.Add(User.PositionHovered);

                //If the selected action automatically picks mobs in the given location. Add them to the usage parameters too.
                if (User.InputActionSelected?.MobFilterParams.PickMobInTargetPos ?? throw new Exception("There is no action selected, how did we get here?"))
                {
                    var mobs_found = Global.ManagerMob.GetInCombat().FilterFromPosition(User.PositionHovered);
                    //Filter invalid mobs.
                    mobs_found = mobs_found.Where(
                            x => User.InputActionSelected.MobFilterParams.IsMobValid(User.TurnUsageParameters, x)
                            ).ToList();
                    
                    //Add them to the MobsTargeted list for the action to use.
                    User.TurnUsageParameters.MobsTargeted.AddRange(mobs_found);
                }
            }
        }

        //Position selection finished.
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
                BattleController.CompActionRunner.QueueAdd(User.InputActionSelected, User.TurnUsageParameters);
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

    private bool HasTargetPositionsRemaining() => User.TurnUsageParameters.PositionsTargeted.Count < User.InputActionSelected.TargetParams.TargetingMaxPositions;


    private void UpdateAoEVisuals()
    {
        if (User.TurnUsageParameters.PositionsTargeted.Count != 0)
        {
            List<Vector3i> aoe_marks = User.InputActionSelected.TargetParams.GetAoEPositions(User.TurnUsageParameters);

            BattleController.CompDisplayGrid.MeshSet(
                aoe_marks, 
                GridNode.Layer.AOE, 
                Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_AOE)
                );
        }
        else
        {
            BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
        }
    }
}
