using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStatePaused : BattleControllerState
{
    public BattleControllerStatePaused(BattleController.State identifier) : base(identifier)
    {
    }

    private BattleControllerState StatePrePause;
    public override void StateOnEnter()
    {
        StatePrePause = User.StatePrevious;
        User.CompCamera.SetControl(false);
        User.CompPauseMenu.AddSceneWithDeclarations(
            Pause.SCENE_PATH,
            Pause.NodesRequired
            );

        if (User.StatePrevious is null)
        {
            throw new Exception("Entered state without a state to return to.");
        }
    }

    public override void StateOnExit()
    {
        User.CompPauseMenu.RemoveSelf();
        User.CompCamera.SetControl(true);
    }

    public override void StateProcess(double delta)
    {
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            if (StatePrePause is BattleControllerState not_null)
            {
                User.FSMSetState(not_null);
            }
            else
            {
                throw new Exception("Entered pause without setting a previous state to return to!");
            }
        }
    }
}
