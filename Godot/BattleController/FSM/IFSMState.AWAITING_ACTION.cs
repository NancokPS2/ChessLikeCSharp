using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateAwaitingAction : BattleControllerState
{
    private PopupButtonDialogUI _popup = new PopupButtonDialogUI().GetInstantiatedScene<PopupButtonDialogUI>();

    public BattleControllerStateAwaitingAction(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        MobCombatUI mob_ui = User.CompMobCombatUI;
        TurnManager turn_manager = User.CompTurnManager;

        Mob taking_turn = turn_manager.GetCurrentTurnTaker() as Mob;

        mob_ui.CompActionMenu.UpdateActionButtons(taking_turn);
        mob_ui.CompActionMenu.EnableActionButtons(true); 

        mob_ui.CompUnitStatus.UpdateStatNodes(taking_turn);
        mob_ui.CompEquipMenu.UpdateEquipment(taking_turn);

    }

    public override void StateOnExit()
    {
        User.CompMobCombatUI.CompActionMenu.EnableActionButtons(false);
    }

    public override void StateProcess(double delta)
    {

        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.PAUSED);
        }

        User.UpdateCursorMovement();
        User.UpdateCameraPosition(delta);

        //If an action was selected, pass to the TARGETING state.
        if (User.InputActionSelected is not null)
        {
            //TODO: Owner cannot be null
            User.TurnUsageParameters = new Action.UsageParams(
                User.CompTurnManager.GetCurrentTurnTaker() as Mob, 
                User.CompGrid, 
                User.InputActionSelected
                );
            User.FSMSetState(BattleController.State.TARGETING);
        }

        User.UpdateMobUI();

        //If the button to end turn was pressed, bring up the popup.
        if (User.InputEndTurnPressed > 0)
        {
            _popup
                .SetMessage("Do you want to end the turn?")
                .Setup<PopupButtonDialogUI.EConfirmCancel>(User);
            User.InputEndTurnPressed = 0;
        }
        
        //If the popup's last index was a valid one.
        if (_popup.IndexLastPressed != PopupButtonDialogUI.NO_INDEX)
        {
            if (_popup.IndexLastPressed == (int)PopupButtonDialogUI.EConfirmCancel.CONFIRM)
            {
                //Swap to ENDING_TURN
                User.FSMSetState(BattleController.State.ENDING_TURN); 
            }
            //else if (_popup.IndexLastPressed == (int)PopupUI.OPTION_CONFIRM_CANCEL.CANCEL)

            _popup.Reload();
        }

    }

}
