using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateTakingTurn : BattleControllerState
{
    public BattleControllerStateTakingTurn(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        User.CompTurnManager.StartTurn();
        User.CompDisplayMob.MobUINode.UpdateDelayList(User.CompTurnManager);
        User.FSMSetState(BattleController.State.AWAITING_ACTION);
    }

    public override void StateOnExit()
    {

    }

    public override void StateProcess(double delta)
    {
    }
}
