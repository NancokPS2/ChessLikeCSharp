using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Turn;

namespace ChessLike.Context;

public class TurnContext : Context
{
	public enum EStep { OWNER, ROUND, FINISHED }
	public event Step<EStep>? StateChanged;

	private ITurn? turnOwner;
	private int round;

	public ITurn? TurnOwner
	{
		get => turnOwner;
		set
		{
			turnOwner = value;
			StateChanged?.Invoke(this, EStep.OWNER);
		}
	}
	public int Round
	{
		get => round;
		set
		{
			round = value;
			StateChanged?.Invoke(this, EStep.ROUND);
		}
	}

	public override void Finish()
	{
		StateChanged?.Invoke(this, EStep.FINISHED);
	}
}
