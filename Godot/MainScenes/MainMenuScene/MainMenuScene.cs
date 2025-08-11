using Godot;
using System;

[GlobalClass]
public partial class MainMenuScene : Control
{
	[Export]
	protected Button PlayButtonNode;

	[Export]
	protected Button LoadButtonNode;

	[Export]
	protected Button QuitButtonNode;

	[Export]
	protected LineEdit SaveIdentifierLineNode;

	public override void _Ready()
	{
		base._Ready();
		PlayButtonNode.Pressed += OnPlayButtonPressed;
		LoadButtonNode.Pressed += OnLoadButtonPressed;
		QuitButtonNode.Pressed += OnQuitButtonPressed;
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}


	private void OnLoadButtonPressed()
	{
		throw new NotImplementedException();
	}

	private void OnPlayButtonPressed()
	{
		UIManager.ChangeToUI(EUIScene.SAVE_SELECT);
	}
}
