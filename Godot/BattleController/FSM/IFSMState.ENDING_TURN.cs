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
    private bool _confirmed;

    public BattleControllerStateEndingTurn(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        User.CompTurnManager.EndTurn();
        //Already updated when a turn starts, this is overdoing it.
        //User.CompCombatUI.Update(User);
        User.FSMSetState(BattleController.State.TAKING_TURN);
    }

    public override void StateOnExit()
    {
    }

    public override void StateProcess(double delta)
    {
    }
}
