using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

public partial class BattleController// : IFSM<BattleController>
{
    public List<IFSMState<BattleController>> StateList { get; set; } = new()
    {

    };
    public float ProcessDelta { get; set; }
    public IFSMState<BattleController> StateCurrentUNUSED { get; set; }

    public void FSMSetup()
    {
        StateCurrentUNUSED = StateList[0];
        foreach (var item in StateList)
        {
            item.User = this;
        }
    }

    public void FSMProcess(double delta)
    {
        StateCurrentUNUSED.StateProcess(delta);
    }
}
