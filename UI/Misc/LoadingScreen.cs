using Godot;
using GodotPlugins.Game;
using System;

[GlobalClass]
public partial class LoadingScreen : Control
{
	[Export]
	protected Light2D LightNode = null!;

	[Export]
	protected Label TimeLabelNode = null!;

	protected Tween MainTween;

	protected double TimeLoading;

	public override void _EnterTree()
	{
		base._EnterTree();
		TimeLoading = 0;
	}


	public override void _Ready()
	{
		base._Ready();
		LightNode.Position = GetRandomScreenPosition();
		GetViewport().GuiFocusChanged += OnGuiFocusChanged;
		StartTween();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		TimeLoading += delta;
		TimeLabelNode.Text = TimeLoading.ToString();
	}

	protected void StartTween()
	{
		MainTween?.Kill();
		MainTween = CreateTween();

		MainTween.TweenProperty(
			LightNode,
			"position",
			GetRandomScreenPosition(),
			(GD.Randf() * 2) + 1
			);
		MainTween.TweenInterval(GD.Randf() * 3);

		MainTween.Finished += StartTween;
	}

	protected Godot.Vector2 GetRandomScreenPosition()
		=> new Godot.Vector2(GetRect().Size.X * GD.Randf(), GetRect().Size.Y * GD.Randf());

	#region Event Handling
	private void OnGuiFocusChanged(Control node)
	{
		if (!IsInsideTree()) return;
		GetViewport().GuiReleaseFocus();
	}
	#endregion
}
