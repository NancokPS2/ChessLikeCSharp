using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateEndingTurn : BattleControllerState
{
    public BattleControllerStateEndingTurn(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        BattleController.CompTurnManager.EndTurn();
    }

    public override void StateOnExit()
    {
    }

    public override void StateProcess(double delta)
    {
        //If any end of turn passives need to be used, wait for the queue to be emptied.
        if (!BattleController.CompActionRunner.QueueIsEmpty())
        {
            return;
        }

        if (BattleController.Encounter.IsEncounterOver())
        {
            User.FSMSetState(BattleController.State.END_COMBAT);
        }

        User.FSMSetState(BattleController.State.TAKING_TURN);
    }
}
