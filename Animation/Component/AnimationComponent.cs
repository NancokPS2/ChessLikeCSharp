using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Animation;

[GlobalClass]
public abstract partial class AnimationComponent : Node3D
{
	protected bool Finished = false;
	
	[Export]
	protected bool StopOnFinished = true;

	public override void _Ready()
	{
		base._Ready();
		if (!IsValidAnimationTarget(GetParent())) SetProcess(false);
		GD.PushError($"Invalid target, cannot animate type {GetParent().GetType()}");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Finished && StopOnFinished) return;
		ProcessAnim((float)delta);
	}

	protected bool IsValidAnimationTarget(Node node) => node is Node3D;
	protected Node3D GetAnimated() => GetParent<Node3D>();
	protected Godot.Vector3 GetAnimatedPosition() => GetAnimated().GlobalPosition;


	protected abstract void ProcessAnim(float delta);
	public virtual bool IsFinished() => Finished;
}
