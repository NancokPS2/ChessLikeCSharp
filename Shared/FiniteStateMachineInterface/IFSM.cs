using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

/// <summary>
/// Una interfaz para implementar una finite state machine.
/// </summary>
/// <typeparam name="TUser">La clase que va a implementar la interfaz.</typeparam>
public interface IFSM<TUser>
{
    public List<IFSMState<TUser>> StateList {get;set;}
    public IFSMState<TUser> StateCurrentUNUSED {get;set;}
    public float ProcessDelta {get;set;}

    public void FSMSetup();

    public void FSMSetState(IFSMState<TUser> state)
    {
        StateCurrentUNUSED.StateOnExit();

        StateCurrentUNUSED = state;

        StateCurrentUNUSED.StateOnEnter();
    }

    public void FSMProcess(double delta)
    {
        StateCurrentUNUSED.StateProcess(delta);
    }

}
