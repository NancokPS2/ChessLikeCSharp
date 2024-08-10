using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IStateMachine
{
    public List<IState> States {get;set;}
    public IState StateCurrent {get;set;}

    public void ProcessState(double delta)
    {
        StateCurrent.Process(delta);
    }

    public void ChangeState(int index)
    {
        if (index < 0 || index >= States.Count)
        {
            throw new ArgumentException("Out of range.");
        }
        StateCurrent.OnExit();

        StateCurrent = States[index];

        StateCurrent.OnEnter();
    }

}

public interface IState
{
    public void Process(double delta);

    public void OnEnter();

    public void OnExit();
}