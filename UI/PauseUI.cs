
using ChessLike.Shared;
using Godot;
using System;

[GlobalClass]
public partial class PauseUI : Control, ISceneDependency
{
    string ISceneDependency.SCENE_PATH { get; } = "res://Godot/Display/UI/Pause.tscn";

	private enum MenuOption {RESUME, PARTY}

	[Export]
	protected PartyGeneralUI PartyUINode;

	[Export]
	protected Button ButtonResume;

	[Export]
	protected Button ButtonParty;

	public override void _Ready()
	{
		base._Ready();

		ButtonResume.Pressed += () => OnButtonPressed(MenuOption.RESUME);
		ButtonParty.Pressed += () => OnButtonPressed(MenuOption.PARTY);

		EventBus.CombatStateChanged += OnCombatStateChanged;

		this.RemoveSelf(true);
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);
		if (@event.IsActionPressed("pause"))
		{
			InputPause();
		}
    }


	#region Event Handling
	private void OnCombatStateChanged(ECombatState obj)
	{
		if (obj == ECombatState.PAUSED && !IsInsideTree())
		{
			this.AddSelf();
		}
		else if (IsInsideTree())
		{
			this.RemoveSelf();
		}
	}

    private void InputPause()
	{
		EventBus.InputPause?.Invoke();
	}

    private void OnButtonPressed(MenuOption button)
	{
		switch (button)
		{
			case MenuOption.RESUME:
				this.RemoveSelf();
				InputPause();
				break;

			case MenuOption.PARTY:
				PartyUINode.ToggleFromScene();
				if (PartyUINode.IsInsideTree()) PartyUINode?.Update();
				break;
			default:
				break;
		}
		
	}
	#endregion
}
