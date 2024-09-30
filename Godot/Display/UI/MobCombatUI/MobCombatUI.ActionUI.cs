using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Action = ChessLike.Entity.Action;

namespace Godot.Display;

public partial class ActionUI
{
    public delegate void ActionPressedEventHandler(Action action);
    public delegate void ButtonPressed();
    public event ActionPressedEventHandler? ActionPressed;
    public event ButtonPressed? EndTurnPressed;



    public VBoxContainer ControlReference;

    public ActionUI(VBoxContainer control_ref)
    {
        ControlReference = control_ref;
    }

    public void UpdateActionButtons(Mob mob)
    {
        VBoxContainer container = ControlReference;
        container.FreeChildren();
        
        foreach (ChessLike.Entity.Action action in mob.GetActions())
        {
            ActionButton button = new(action);
            container.AddChild(button);

            button.Text = action.Name;
            Console.WriteLine(button.GetPath());
            button.Pressed += () => ActionPressed?.Invoke(action);
        }

        //Button for ending turn.
        Button end_turn = new();
        end_turn.Pressed += () => EndTurnPressed.Invoke();
        end_turn.Text = "End Turn";
        container.AddChild(end_turn);
    }

    public void EnableActionButtons(bool enable)
    {
        foreach (Node node in ControlReference.GetChildren())
        {
            if (node is Button button)
            {
                button.Disabled = !enable;
            }
        }
    }


    private partial class ActionButton : Button
    {
        public Action action;

        public ActionButton(Action action)
        {
            this.action = action;
            Text = action.Name;
        }
    }
}
