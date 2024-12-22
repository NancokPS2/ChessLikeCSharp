using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Turn;
using ExtendedXmlSerializer;
using Godot.Display;
using Sprache;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateTargeting : BattleControllerState
{
    private List<Vector3i> _last_param_positions = new();
    private PopupButtonDialogUI _popup = new PopupButtonDialogUI().GetInstantiatedScene<PopupButtonDialogUI>();
    private List<Vector3i> _pos_valid_for_targeting = new();
    private UniqueList<Vector3i> PositionsSelected = new();

    public BattleControllerStateTargeting(BattleController.State identifier) : base(identifier)
    {
    }


    public override void StateOnEnter()
    {
        ResetTargetingSelections();
        _pos_valid_for_targeting = GetSelectedAbility().TargetParams
            .GetTargetedPositions(GetUsageParameters());

        BattleController.CompDisplayGrid.MeshSet(
            GetPositionsWithinRange(), 
            GridNode.Layer.TARGETING, 
            Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_TARGETABLE)
            );
    }

    public override void StateOnExit()
    {
        
        User.PositionSelected = Vector3i.INVALID;
        _last_param_positions = new();

        //If not about to run the action or to pause, also reset the targeting values.
        if (User.StateCurrent is not BattleControllerStateActionRunning && User.StateCurrent is not BattleControllerStatePaused)
        {
            User.InputActionSelected = null;
            ResetTargetingSelections();
        }
        PositionsSelected.Clear();
        _pos_valid_for_targeting = new();
        
        //Clean visuals
        BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
        BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
    }

    public override void StateProcess(double delta)
    {
        //Setup values
        ActionEvent.UsageParameters usage_params = User.TurnUsageParameters ?? throw new Exception("No UsageParametes set yet.");
        Ability ability_selected = User.InputActionSelected ?? throw new Exception("No Ability was selected yet.");
        Debug.Assert(GetSelectedAbility() == GetUsageParameters().ActionRef, "The UsageParameters should point to the selected action.");

        //Pause menu
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.PAUSED);
        }

        //Handle AoE displaying when changing the hovered positions.
        if (usage_params.PositionsTargeted.Count != 0 && usage_params.PositionsTargeted != _last_param_positions)
        {
            UpdateTargetedVisuals();
            _last_param_positions = usage_params.PositionsTargeted;
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
            //If can still select positions, add it to the list.
            if (HasTargetPositionsRemaining() 
                && GetPositionsWithinRange().Contains(User.PositionHovered)
                )
            {
                //Add the position selected to the List.
                PositionsSelected.Add(User.PositionHovered);
            }
        }

        //Position selection finished.
        if (!HasTargetPositionsRemaining())
        {
            //Show the popup to confirm if it hasn't yet.
            if (_popup.GetParent() is null && _popup.IndexLastPressed == PopupButtonDialogUI.NO_INDEX)
            {
                _popup
                    .SetMessage("Confirm action?")
                    .Setup<PopupButtonDialogUI.EConfirmCancel>(User);
            }

            //If confirmed, change the state.
            if (_popup.IndexLastPressed == (int)PopupButtonDialogUI.EConfirmCancel.CONFIRM)
            {
                //Update the usage parameters with the ones targeted.
                AddTargetedToUsageParameters();
                //Add action to the queue
                BattleController.CompActionRunner.QueueAdd(
                    GetSelectedAbility(), 
                    GetUsageParameters()
                    );
                User.FSMSetState(BattleController.State.ACTION_RUNNING);
                UpdateTargetedVisuals(true);
                _popup.Reload();
            }

            //Cancel the selected positions.
            else if (_popup.IndexLastPressed == (int)PopupButtonDialogUI.EConfirmCancel.CANCEL)
            {
                ResetTargetingSelections();
                UpdateTargetedVisuals(true);
                _popup.Reload();
            }
        }
    }

    /// <summary>
    /// Should be called before proceeding to use the action.
    /// </summary>
    public void AddTargetedToUsageParameters()
    {
        //Add the AoE positions steming from SELECTED ones.
        UniqueList<Vector3i> positions_to_add = new(){Safe = false};
        positions_to_add.AddRange(
            GetSelectedAbility().TargetParams.GetAoEPositions(
                GetUsageParameters(), PositionsSelected
                )
            );
        GetUsageParameters().PositionsTargeted = positions_to_add;

        //If mobs cannot be considered, stop here.
        if (!GetSelectedAbility().MobFilterParams.PickMobInTargetPos){return;}   

        //Add the targeted mobs to the UsageParameters if valid.
        List<Mob> mobs_found = new();
        foreach (var pos in GetUsageParameters().PositionsTargeted)
        {      
                //Get the mobs at this position.
                List<Mob> mobs_here = Global.ManagerMob
                    .GetInCombat()
                    .FilterFromPosition(pos);
                
                List<Mob> mobs_filtered = new();
                foreach (var mob in mobs_here)
                {
                    if (GetSelectedAbility().MobFilterParams.IsMobValid(GetUsageParameters(), mob))
                    {
                        mobs_filtered.Add(mob);            
                    }
                }

                //Add filtered mobs to the MobsTargeted list for the action to use.
                GetUsageParameters().MobsTargeted.AddRange(mobs_filtered);
        }
    }

    public void ResetTargetingSelections()
    {
        GetUsageParameters().PositionsTargeted.Clear();   
        GetUsageParameters().MobsTargeted.Clear();   
        PositionsSelected.Clear();
    }

    private bool HasTargetPositionsRemaining() => PositionsSelected.Count < GetSelectedAbility().TargetParams.TargetingMaxPositions;

    private void UpdateTargetedVisuals(bool force_clear = false)
    {
        if (GetUsageParameters().PositionsTargeted.Count != 0 && !force_clear)
        {
            List<Vector3i> targeted_marks = GetUsageParameters().PositionsTargeted;

            BattleController.CompDisplayGrid.MeshSet(
                targeted_marks, 
                GridNode.Layer.AOE, 
                Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_AOE)
                );
        }
        else
        {
            BattleController.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
        }
    }

    public Ability GetSelectedAbility() => User.InputActionSelected ?? throw new Exception("No Ability was selected yet.");
    public ActionEvent.UsageParameters GetUsageParameters() => User.TurnUsageParameters ?? throw new Exception("No Ability was selected yet.");
    public List<Vector3i> GetPositionsWithinRange() => _pos_valid_for_targeting;
}
