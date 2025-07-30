using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;

namespace Godot.Display;

[GlobalClass]
public partial class CombatTurnUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatTurnUI.tscn";

    [Export]
    public Control? NodeTurnContainer;

    protected Dictionary<Mob, Control> TurnDisplayInstances = new();

    public override void _Ready()
    {
        EventBus.MobStateChanged += OnMobStateChanged;
        EventBus.MobTurnStarted += OnMobTurnStarted;
    }


    protected void UpdateInstancePool(Mob mob, bool entering)
	{
		if (NodeTurnContainer is null) throw new Exception();

		if (!TurnDisplayInstances.ContainsKey(mob) && entering)
		{
			DelayContainer newInstance = new() { User = mob };
			TurnDisplayInstances.Add(
				mob,
				newInstance
				);
			NodeTurnContainer.AddChild(newInstance);
		}
		else if (TurnDisplayInstances.ContainsKey(mob) && !entering)
		{
			Control instance = TurnDisplayInstances[mob];
			TurnDisplayInstances.Remove(mob);
			NodeTurnContainer.RemoveChild(instance);
		}
	}

    protected void UpdateCurrentTurnTaker(Mob mob)
    {
        if (!TurnDisplayInstances.ContainsKey(mob)) throw new Exception();
        foreach (var entry in TurnDisplayInstances)
        {
            if (entry.Key == mob)
            {
                entry.Value.Modulate = new(0.8f, 1, 0.8f);
            }
            else
            {
                entry.Value.Modulate = Colors.White;
            }
        }
    }

    #region Event Handling
    private void OnMobTurnStarted(Mob mob)
    {
        UpdateCurrentTurnTaker(mob);
    }

    private void OnMobStateChanged(Mob mob, EMobState state)
    {
        if (state == EMobState.COMBAT)
        {
            UpdateInstancePool(mob, true);

        }
        else if (state == EMobState.BENCHED)
        {
            UpdateInstancePool(mob, false);
        }
    }
	#endregion

	private partial class DelayContainer : TextureRect
	{
		public required ITurn User;

		private Label? label_name;

		private Label? label_delay;

		public DelayContainer()
		{
		}

		public override void _Ready()
		{
			base._Ready();
			SetAnchorsPreset(LayoutPreset.FullRect);
			SizeFlagsHorizontal = SizeFlags.ExpandFill;

			//Label and name
			label_name = new();
			AddChild(label_name);
			label_name.SetAnchorsPreset(LayoutPreset.TopWide);

			label_delay = new();
			AddChild(label_delay);
			label_delay.SetAnchorsPreset(LayoutPreset.BottomWide);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			(label_delay ?? throw new Exception())
				.Text = User.DelayCurrent.ToString();

			if (User is Mob mob)
				(label_name ?? throw new Exception())
					.Text = mob.DisplayedName;
		}


    }
}
