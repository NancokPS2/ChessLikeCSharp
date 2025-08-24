using ChessLike.Extension;
using Godot;
using GodotPlugins.Game;
using System;
using System.Data;

[GlobalClass]
public partial class LoadingScreen : Control
{
	public static LoadingScreen Instance = null!;
	[Export]
	protected Light2D LightNode = null!;

	[Export]
	protected Label TimeLabelNode = null!;

	protected Tween MainTween;

	protected double TimeLoading;

	protected bool IsLoading;

	protected static List<Object> Loading = new();

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
		LightNode.Position = GetRandomScreenPosition();
		GetViewport().GuiFocusChanged += OnGuiFocusChanged;
		StartTween();

		EventBus.CombatPreparationStarted += OnCombatPreparationStarted;
	}

	public static async void SetLoading(ELoadingReason reason, bool loading)
	{
		if (loading && !Loading.Contains(reason))
		{
			Loading.Add(reason);
		}
		else
		{
			Loading.Remove(reason);
		}
		Instance.UpdateScreen(1.0 / 60.0);

		await Instance.ToSignal(Instance.GetTree(), SceneTree.SignalName.ProcessFrame);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Loading.RemoveAll(x => x is null);
		UpdateScreen(delta);
	}

	private void UpdateScreen(double delta)
	{
		IsLoading = Loading.Count != 0;

		//Visible = isLoading;
		MouseFilter = IsLoading ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
		if (IsLoading)
		{
			Modulate = Colors.White;
			TimeLabelNode.Text = $"Time: {TimeLoading / 1000.0}\nLoading:\n{Loading.ToStringList("\n")}";
			TimeLoading += delta;
		}
		else
		{
			Modulate -= Colors.White * (float)delta;
			TimeLoading = 0;
		}

		Modulate = Modulate.Clamp(new(0,0,0,0), Colors.White);
		LightNode.Energy = 3 * Modulate.Luminance;
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
		if (!IsLoading) return;
		GetViewport().GuiReleaseFocus();
	}

	private void OnCombatPreparationStarted()
	{
		SetLoading(ELoadingReason.CHANGING_SCENE, false);
	}
	#endregion
}
