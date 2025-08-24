using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public struct MovementParameters
{
	public EMovementMode MovementMode;

	public MovementParameters(EMovementMode movementMode)
	{
		MovementMode = movementMode;
	}
}
