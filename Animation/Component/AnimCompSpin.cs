using ChessLike.Animation;
using Godot;
using System;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimCompSpin : AnimationComponent
{
	[Export(PropertyHint.Range, "0,100,0.05")]
	public float Speed = 1;

	[Export]
	public Godot.Vector3 Axis = Godot.Vector3.Up;

	public override void _Ready()
	{
		base._Ready();
	}


	protected override void ProcessAnim(float delta)
	{
		GetAnimated().Rotate(Axis, Mathf.Tau * Speed * delta);
	}
}
