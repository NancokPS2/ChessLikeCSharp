using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimationComponentController : Node3D
{
	public bool AutoFreeOnFinish;

	public List<AnimationComponent> Components = new();

	public AnimationComponentController()
	{
	}

	public AnimationComponentController(List<AnimationComponent> components)
	{
		Components = components;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Components.All(x => x.IsFinished()) && AutoFreeOnFinish)
			Components.ForEach( x => x.QueueFree());
	}
}
