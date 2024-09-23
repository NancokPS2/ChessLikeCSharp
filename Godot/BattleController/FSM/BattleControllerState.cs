using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public abstract class BattleControllerState
{
    public BattleController User { get; set; }
    public BattleController.State StateIdentifier = BattleController.State.INVALID;

    public BattleControllerState(BattleController.State identifier)
    {
        StateIdentifier = identifier;
    }

    public abstract void StateOnEnter();

    public abstract void StateOnExit();

    public abstract void StateProcess(double delta);
}
