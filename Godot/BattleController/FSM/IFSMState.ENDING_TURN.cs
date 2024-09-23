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
        User.CompTurnManager.EndTurn();
        User.CompDisplayMob.MobUINode.UpdateDelayList(User.CompTurnManager);
        User.FSMSetState(BattleController.State.TAKING_TURN);
    }

    public override void StateOnExit()
    {
    }

    public override void StateProcess(double delta)
    {
    }
}
