using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Animation;
using Godot;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimationComponentController : Node3D
{
	public delegate void Event();
	public event Event Finished;

	public double DurationMax = 0;
	protected double TimePassed;
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

		if (AutoFreeOnFinish && DurationMax <= 0 && IsAllFinished())
		{
			FreeAll();
			Finished.Invoke();
		}

		else if (DurationMax > 0 && TimePassed > DurationMax)
		{
			FreeAll();
			Finished.Invoke();
		}

		TimePassed += delta;
	}

	private void FreeAll()
	{
		Components.ForEach(x => x.QueueFree());
		QueueFree();
	}

	public bool IsAllFinished() => Components.All(x => x.IsFinished());
}
