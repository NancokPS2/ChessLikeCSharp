using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Context;

public class MobMoveContext : Context
{
	public enum EStep {MOVER, MODE, PATH}
	public event Step<EStep>? StepDone;

	private Mob? mover;
	private List<Vector3i> path = new();

	public Mob? Mover
	{
		get => mover;
		set
		{
			mover = value;
			StepDone?.Invoke(this, EStep.MOVER);
		}
	}

	public List<Vector3i> Path
	{
		get => path;
		set
		{
			path = value;
			StepDone?.Invoke(this, EStep.PATH);
		}
	}

	public override void Finish()
	{
		throw new NotImplementedException();
	}
}
