using Godot;
using System;
using Vector3 = Godot.Vector3;

namespace ChessLike.Animation;

[GlobalClass]
public partial class AnimCompFaceTravelDirection : AnimationComponent
{
	[Export(PropertyHint.Range, "0,1,0.01")]
	protected float Rigidness = 1;

	protected Vector3 PosPreviousGlobal;

	public override void _Ready()
	{
		base._Ready();
		PosPreviousGlobal = GetAnimated().GlobalPosition;
		Finished = true;
	}

	protected override void ProcessAnim(float delta)
	{
		GetAnimated().LookAt(
			GetAnimatedPosition() + ((GetAnimatedPosition() - PosPreviousGlobal) * Rigidness)
			);
	}

	protected override bool IsPerpetual() => true;

}
