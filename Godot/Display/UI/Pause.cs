
using ChessLike.Shared;
using Godot;
using System;

[GlobalClass]
public partial class Pause : Control, ISceneDependency
{
    string ISceneDependency.SCENE_PATH { get; } = "res://Godot/Display/UI/Pause.tscn";

	private enum MenuOption {RESUME, PARTY}

	private PartyGeneralUI _party_scene;

	[Export]
	public Button ButtonResume;
	[Export]
	public Button ButtonParty;

	public Pause()
	{
		_party_scene = new PartyGeneralUI().GetInstantiatedScene<PartyGeneralUI>();
		_party_scene.Activate(false);
	}

    public override void _Ready()
    {
        base._Ready();
		AddChild(_party_scene);

		ButtonResume.Pressed += () => OnButtonPressed(MenuOption.RESUME);
		ButtonParty.Pressed += () => OnButtonPressed(MenuOption.PARTY);
    }

	private void OnButtonPressed(MenuOption button)
	{
		switch (button)
		{
			case MenuOption.RESUME:
				this.RemoveSelf();
				break;
			case MenuOption.PARTY:
				_party_scene.Activate(true);
				_party_scene.Update();
				break;
			default:
				break;
		}
		
	}

}
