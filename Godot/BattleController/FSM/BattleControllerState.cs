using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public abstract class BattleControllerState
{
    public BattleController User { get; set; }
    public EBattleState StateIdentifier = EBattleState.INVALID;

    public BattleControllerState(EBattleState identifier)
    {
        StateIdentifier = identifier;
    }

    public abstract void StateOnEnter();

    public abstract void StateOnExit();

    public abstract void StateProcess(double delta);
}
