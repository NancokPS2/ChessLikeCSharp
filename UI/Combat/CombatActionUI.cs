using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.UI.Base;
using ChessLike.World.Encounter;
using Godot;
using System;

[GlobalClass]
public partial class CombatActionUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatActionUI.tscn";

    [Export]
    public Control? NodeActionContainer;

    private Mob? MobCurrent;

    protected Mob? MobSelected;
    protected Mob? MobToUpdate;

    public CombatActionUI()
    {
        EventBus.BattleStateChanged += OnBattleStateChanged;
        EventBus.MobTurnStarted += OnMobTurnStarted;
        EventBus.MobSelected += OnMobSelected;
    }

	public override void _Ready()
    {
        base._Ready();
        NodeActionContainer ??= (Control)FindChild("ActionContainer");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Visible = MobSelected == MobCurrent;
        if (NodeActionContainer is null) throw new Exception();

        //If there are no buttons to select, end here.
        if (NodeActionContainer.GetChildren().Count == 0) return;

        //If there are buttons but there is no mob, something went wrong.
        if (MobCurrent is null) throw new Exception("Buttons where generated but the mob that owns them is gone.");
		
		//Enable or disable the input to buttons.
        foreach (ActionButton item in NodeActionContainer.GetChildren().Where(x => x is ActionButton))
		{
			item.Disabled = !item.HasMobEnoughResources(MobCurrent);
		}
    }

    public void Update(Mob mob)
    {
        MobCurrent = mob;

        UpdateActionButtons(mob);
    }

    public void UpdateActionButtons(Mob mob)
    {
        if (NodeActionContainer is null) { throw new Exception("Null NodeActionContainer"); }

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
        ConfirmationButton end_turn = new();
        end_turn.Confirmed += () => EventBus.InputTurnEnded?.Invoke();
        end_turn.Text = "End Turn";
        container.AddChild(end_turn);
    }

    private void EnableActionButtons(bool enable)
    {
        if (NodeActionContainer is null) { throw new Exception("Null NodeActionContainer"); }

        foreach (Node node in NodeActionContainer.GetChildren())
        {
            if (node is Button button)
            {
                button.Disabled = !enable;
            }
        }
    }

    #region Event Handling
    private void OnBattleStateChanged(EBattleState state)
    {
        switch (state)
        {
            case EBattleState.ACTION_INPUT:
                EnableActionButtons(true);
                break;

            default:
                EnableActionButtons(false);
                break;
        }
    }

    private void OnMobTurnStarted(Mob mob)
    {
        Update(mob);
    }

    private void OnMobSelected(Mob obj)
    {
        MobSelected = obj;
    }
	#endregion

	#region ActionButton subclass
	private partial class ActionButton : Button
	{
		public Ability action;

		public ActionButton(Ability action)
		{
			this.action = action;
			Text = action.Name;
		}

		public bool HasMobEnoughResources(Mob mob)
		{
			bool enoughActions = mob.Stats.GetValue(EValueName.ACTION) >= action.CostParams.Action;
			bool enoughReactions = mob.Stats.GetValue(EValueName.ACTION) >= action.CostParams.Reaction;
			return enoughActions && enoughReactions;
		}
    }
    #endregion
}
