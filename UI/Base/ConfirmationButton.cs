using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.UI.Base;

[GlobalClass]
public partial class ConfirmationButton : Button
{
    public delegate void Confirmation();
    public event Confirmation? Confirmed;

    private bool primed;

    protected bool Primed
    {
        get => primed;
        set
        {
            primed = value;
            _Primed(primed);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        GetViewport().GuiFocusChanged += OnGuiFocusChanged;
    }

    public override void _Pressed()
    {
        base._Pressed();
        if (Primed)
        {
            Primed = false;
            Confirmed?.Invoke();
        }
        else Primed = true;
    }


    public virtual void _Primed(bool primed)
    {
        Modulate = primed ? Colors.Green : Colors.White;
    }

    private void OnGuiFocusChanged(Control node)
    {
        if (node != this) Primed = false;
    }
}
