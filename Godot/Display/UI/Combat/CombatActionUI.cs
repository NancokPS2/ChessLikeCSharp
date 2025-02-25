using ChessLike.Entity;
using ChessLike.Entity.Action;
using Godot;
using System;

public partial class CombatActionUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatActionUI.tscn";

	[Export]
	public Control? NodeActionContainer;

	private Mob? MobCurrent;

    public CombatActionUI()
    {
        EventBus.BattleStateChanged += OnBattleStateChanged;
    }

    private void OnBattleStateChanged(BattleControllerState obj)
    {
        switch(obj.StateIdentifier)
        {
            case BattleController.State.AWAITING_ACTION:
                EnableActionButtons(true);
                break;

            default:
                EnableActionButtons(false);
                break;
        }
    }

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
        
        foreach (Ability action in mob.GetAbilities())
        {
            ActionButton button = new(action);
            container.AddChild(button);

            button.Text = action.Name;
            Console.WriteLine(button.GetPath());
            button.Pressed += () => EventBus.InputActionSelected?.Invoke(action);
        }

        //Button for ending turn.
        Button end_turn = new();
        end_turn.Pressed += () => EventBus.InputTurnEnded?.Invoke();
        end_turn.Text = "End Turn";
        container.AddChild(end_turn);
    }

    private void EnableActionButtons(bool enable)
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
        public Ability action;

        public ActionButton(Ability action)
        {
            this.action = action;
            Text = action.Name;
        }
    }
}
