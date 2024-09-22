using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot;

/// <summary>
/// Un estado para una IFSM (Interface Finite State Machine). Tanto la IFSM como la IFSMState deben especificar la clase con la cual trabajan.
/// </summary>
/// <typeparam name="TUser">La clase que usa la IFSM y por lo tanto actua como la state machine que contiene al estado.</typeparam>
public interface IFSMState<TUser>
{
    /// <summary>
    /// El usuario de la state machine. Por lo general deberia ser la IFSM que contiene a estos estados.
    /// </summary>
    /// <value></value>
    public TUser User {get;set;}

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Si el estado puede usarse ahora mismo.</returns>
    public bool StateCanEnter()
    {
        return true;
    }

    public bool StateCanExit()
    {
        return true;
    }
    
    public void StateOnEnter();

    public void StateOnExit();

    public void StateProcess(double delta);

}
