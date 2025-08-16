
using ChessLike.Shared;
using ChessLike.UI.Base;
using Godot;
using System;

[GlobalClass]
public partial class PauseUI : Control, ISceneDependency
{
    string ISceneDependency.SCENE_PATH { get; } = "res://Godot/Display/UI/Pause.tscn";

	[Export]
	protected Button ButtonResume;

	[Export]
	protected Button ButtonParty;

	[Export]
	protected Button ButtonSave;

	[Export]
	protected ConfirmationButton ButtonQuit;

	public override void _Ready()
	{
		base._Ready();

		ButtonResume.Pressed += () => OnButtonPressed(EPauseOption.RESUME);
		ButtonParty.Pressed += () => OnButtonPressed(EPauseOption.PARTY);
		ButtonSave.Pressed += () => OnButtonPressed(EPauseOption.SAVE);
		ButtonQuit.Confirmed += () => OnButtonPressed(EPauseOption.QUIT);
	}


	#region Event Handling
    private void OnButtonPressed(EPauseOption button)
	{
		if (button == EPauseOption.RESUME) this.RemoveSelf();

		EventBus.InputPauseOptionSelected?.Invoke(button);	
	}
	#endregion
}
