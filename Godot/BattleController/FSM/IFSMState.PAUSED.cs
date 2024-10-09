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

    private Pause? _menu_reference;
    private BattleControllerState? StatePrePause;
    public BattleControllerStatePaused(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        StatePrePause = User.StatePrevious;

        User.CompCamera.SetControl(false);

        _menu_reference = new Pause().GetInstantiatedScene<Pause>();
        User.CompCanvas.AddChild(_menu_reference);

        if (User.StatePrevious is null)
        {
            throw new Exception("Entered state without a state to return to.");
        }
    }

    public override void StateOnExit()
    {
        if (_menu_reference is not null){_menu_reference.RemoveSelf();}
        User.CompCamera.SetControl(true);
    }

    public override void StateProcess(double delta)
    {
        bool pause_pressed = Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE); 
        bool menu_null = _menu_reference is null;

        if (
            pause_pressed
            || menu_null
            || _menu_reference.GetParent() is null
            )
        {
            ReturnToPreviousState();
        }
    }

    public void ReturnToPreviousState()
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
