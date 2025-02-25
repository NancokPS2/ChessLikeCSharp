using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Turn;
using Godot.Display;

namespace Godot;


public class BattleControllerStateAwaitingAction : BattleControllerState
{
    private int endTurnPressed;

    private PopupButtonDialogUI _popup = new PopupButtonDialogUI().GetInstantiatedScene<PopupButtonDialogUI>();

    public BattleControllerStateAwaitingAction(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        EventBus.InputTurnEnded += OnInputTurnEnded;

        CombatGeneralUI mob_ui = BattleController.CompCombatUI;

        mob_ui.Update(User);

        EventBus.InputTurnEnded += () => OnTurnEnd();

    }

    public override void StateOnExit()
    {
        EventBus.InputTurnEnded -= OnInputTurnEnded;
    }

    private void OnInputTurnEnded() => endTurnPressed ++;


    private int OnTurnEnd()
    {
        return endTurnPressed++;
    }



    public override void StateProcess(double delta)
    {
        //Can switch to PAUSE state from here.
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.PAUSED);
        }

        User.UpdateCursorMovement();
        User.UpdateCameraPosition(delta);

        //If an action was selected, pass to the TARGETING state.
        if (User.ActionSelected is not null)
        {
            //
            //TODO: Owner cannot be null
            User.TurnUsageParameters = new Ability.UsageParameters(
                BattleController.CompTurnManager.GetCurrentTurnTaker() as Mob, 
                BattleController.CompGrid, 
                User.ActionSelected
                );
            User.FSMSetState(BattleController.State.TARGETING);
        }

        User.UpdateHoveredMobUI();

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
