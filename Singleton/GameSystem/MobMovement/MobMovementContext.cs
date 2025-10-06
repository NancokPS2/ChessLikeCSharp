using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Context;

public class MobMovementContext : Context
{
	public enum EStep {
		MOVER,
		MODE,
		PATH,
	}
	public event Step<EStep>? StepDone;

	private Mob? mover;
	private EMovementMode movementMode;
	private List<Vector3i> path = new();

	public MobMovementContext(Mob? mover, EMovementMode movementMode, List<Vector3i> path)
	{
		Mover = mover;
		MovementMode = movementMode;
		Path = path;
	}

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

	public EMovementMode MovementMode
	{
		get => movementMode;
		set
		{
			movementMode = value;
			StepDone?.Invoke(this, EStep.MODE);
		}
	}

	public void Finish()
	{
		throw new NotImplementedException();
	}
}
