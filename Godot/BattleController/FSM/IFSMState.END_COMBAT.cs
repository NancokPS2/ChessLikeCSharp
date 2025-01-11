using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using ChessLike.World.Encounter;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateEndCombat : BattleControllerState
{
    public BattleControllerStateEndCombat(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        MessageQueue.AddMessage("VICTORY");
        BattleController.Encounter.Finish();
    }

    public override void StateOnExit()
    {
    }

    public override void StateProcess(double delta)
    {

    }
}
