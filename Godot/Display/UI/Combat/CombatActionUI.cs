using ChessLike.Entity;
using Godot;
using System;
using Action = ChessLike.Entity.Action;

public partial class CombatActionUI : Control, ISceneDependency
{
    public delegate void ActionPressedEventHandler(Action action);
    public delegate void ButtonPressed();
    public event ActionPressedEventHandler? ActionPressed;
    public event ButtonPressed? EndTurnPressed;

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatActionUI.tscn";

	[Export]
	public Control? NodeActionContainer;

	private Mob MobCurrent;

    public override void _Ready()
    {
        base._Ready();
		NodeActionContainer ??= (Control)FindChild("ActionContainer");
    }

    public void Update(Mob mob)
	{
		MobCurrent = mob;

		UpdateActionButtons(mob);
	}

    public void UpdateActionButtons(Mob mob)
    {
		if(NodeActionContainer is null) {throw new Exception("Null NodeActionContainer");}

        Control container = NodeActionContainer;
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
		if(NodeActionContainer is null) {throw new Exception("Null NodeActionContainer");}

        foreach (Node node in NodeActionContainer.GetChildren())
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
