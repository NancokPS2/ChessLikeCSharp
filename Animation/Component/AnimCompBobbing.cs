using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimCompBobbing : AnimationComponent
{
	[Export]
	public float TrajectoryPeak = 1;

	[Export]
	public float Duration = 1;

	protected double TimePassed;

	protected override void ProcessAnim(float delta)
	{
		if (TimePassed < Duration / 2)
			GetAnimated().GlobalPosition += (Vector3.Up * TrajectoryPeak * delta) / Duration;
		else
			GetAnimated().GlobalPosition -= (Vector3.Up * TrajectoryPeak * delta) / Duration;

		if (TimePassed > Duration) Finished = true;
		TimePassed += delta;
	}
	protected override bool IsPerpetual() => false;
}
