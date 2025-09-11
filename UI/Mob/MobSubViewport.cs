using ChessLike.Entity;
using Godot;
using System;

/// <summary>
/// Shows the currently selected mob in its view.
/// </summary>
[GlobalClass]
public partial class MobSubViewport : SubViewportContainer
{
	private const uint LAYER_ALL = 0xFFFFF;
	private const uint LAYER_BASE = (uint)Readonly.Values.VisualInstance3DLayer.BASE;
	private const uint LAYER_MOB_VIEW = (uint)Readonly.Values.VisualInstance3DLayer.MOB_SUB_VIEW;

	[Export]
	protected Node3D CameraOriginNode = null!;

	[Export]
	protected Camera3D CameraNode = null!;

	[Export]
	protected Viewport ViewportNode = null!;

	[Export]
	protected bool ShowIsolated = false;

	protected MobScene? MobSceneLastShown;

	public override void _Ready()
	{
		base._Ready();
		EventBus.MobSelected += OnMobSelected;
	}

	protected void ShowMob(MobScene scene)
	{
		//Prevent the previous mob from appearing in the "isolated" view.
		if (MobSceneLastShown is not null && IsInstanceValid(MobSceneLastShown))
			ToggleExclusiveLayer(MobSceneLastShown, false);
			
		//Update position
		CameraOriginNode.GlobalPosition = scene.GlobalPosition;

		//Update the camera's cull mask
		CameraNode.CullMask = ShowIsolated ? LAYER_MOB_VIEW : LAYER_ALL;

		if (ShowIsolated)
		{
			ToggleExclusiveLayer(scene, true);
		}

		MobSceneLastShown = scene;
	}

	protected void ToggleExclusiveLayer(Node3D node, bool enable)
	{
		foreach (var item in node.GetChildrenRecursive())
		{
			if (item is VisualInstance3D visInstance)
				visInstance.Layers = enable ? LAYER_MOB_VIEW | LAYER_BASE : LAYER_BASE;
		}
	}


	protected void OnMobSelected(Mob obj)
	{
		if (MobSceneManager.GetMobScene(obj) is MobScene mobScene)
			ShowMob(mobScene);
	}

}
