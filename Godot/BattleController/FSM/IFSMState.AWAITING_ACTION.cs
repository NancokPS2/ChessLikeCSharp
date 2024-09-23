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
        MobDisplay display_mob = User.CompDisplayMob;
        TurnManager turn_manager = User.CompTurnManager;

        display_mob.MobUINode.EnableActionButtons(true); 
        display_mob.MobUINode.UpdateStatNodes(turn_manager.GetCurrentTurnTaker() as Mob);
        display_mob.MobUINode.UpdateActionButtons(turn_manager.GetCurrentTurnTaker() as Mob);
    }

    public override void StateOnExit()
    {
        User.CompDisplayMob.MobUINode.EnableActionButtons(false);
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
