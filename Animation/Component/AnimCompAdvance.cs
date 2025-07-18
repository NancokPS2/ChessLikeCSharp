using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;
using Vector3 = Godot.Vector3;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimCompAdvance : AnimationComponent
{
	[Export]
	public Godot.Vector3 TargetGlobal;

	[Export]
	public float Duration = 1;

	protected Godot.Vector3 moveVector;
	protected Godot.Vector3 startingPosition;
	protected float TimePassed;

	public override void _Ready()
	{
		base._Ready();
		moveVector = TargetGlobal - GetAnimatedPosition();
		startingPosition = GetAnimatedPosition();
	}

	protected override void ProcessAnim(float delta)
	{
		GetAnimated().GlobalPosition += (moveVector / Duration) * delta;
		
		if (TimePassed > Duration) Finished = true;
		TimePassed += delta;
	}

	protected override bool IsPerpetual() => false;

}
