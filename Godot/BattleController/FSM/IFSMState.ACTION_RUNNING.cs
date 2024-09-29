using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateActionRunning : BattleControllerState
{
    public BattleControllerStateActionRunning(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        if (User.CompActionRunner.IsQueueEmpty()){throw new Exception("Entered state without queued actions.");}

        User.CompMobCombatUI.Hide();
        User.CompActionRunner.RunStart();
    }

    public override void StateOnExit()
    {
        User.CompMobCombatUI.Show();
        User.InputActionSelected = null;
        User.TurnUsageParameters.PositionsTargeted = new();
    }

    public override void StateProcess(double delta)
    {
        if (User.CompActionRunner.IsQueueEmpty())
        {
            User.FSMSetState(BattleController.State.AWAITING_ACTION);
        }
    }
}
