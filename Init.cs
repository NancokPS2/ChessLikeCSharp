using ChessLike.Singleton;
using Godot;
using System;

public partial class Init : Node
{
	public override void _Ready()
	{
		base._Ready();

		SceneChanger.ChangeToMainMenu();

		QueueFree();
	}
}
