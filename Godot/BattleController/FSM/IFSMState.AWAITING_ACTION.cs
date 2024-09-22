using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerState : IFSMState<BattleController>
{
    public BattleController User { get; set; }

    public void StateOnEnter()
    {
        MobDisplay display_mob = User.CompDisplayMob;
        TurnManager turn_manager = User.CompTurnManager;

        display_mob.MobUINode.EnableActionButtons(true); 
        display_mob.MobUINode.UpdateStatNodes(turn_manager.GetCurrentTurnTaker() as Mob);
        display_mob.MobUINode.UpdateActionButtons(turn_manager.GetCurrentTurnTaker() as Mob);
    }

    public void StateOnExit()
    {
        User.CompDisplayMob.MobUINode.EnableActionButtons(false);
    }

    public void StateProcess(double delta)
    {
                        //If an action was selected, pass to the TARGETING state.
        if (User.InputActionSelected is not null)
        {
            //TODO: Owner cannot be null
            User.TurnUsageParameters = new Action.UsageParams(
                User.CompTurnManager.GetCurrentTurnTaker() as Mob, 
                User.CompGrid, 
                User.InputActionSelected
                );
            User.SetState(BattleController.State.TARGETING);
        }
        User.UpdateCameraPosition(delta);
        User.UpdateMobUI();

        //If the button to end turn was pressed, swap to ENDING_TURN
        if (User.InputEndTurnPressed > 0)
        {
            User.SetState(BattleController.State.ENDING_TURN); 
            User.InputEndTurnPressed = 0;
        }
    }
}
