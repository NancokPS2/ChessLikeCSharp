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
        User.UpdateCameraPosition(delta);
        User.UpdateMobUI();

        //If the button to end turn was pressed, swap to ENDING_TURN
        if (User.InputEndTurnPressed > 0)
        {
            User.FSMSetState(BattleController.State.ENDING_TURN); 
            User.InputEndTurnPressed = 0;
        }
    }
}
