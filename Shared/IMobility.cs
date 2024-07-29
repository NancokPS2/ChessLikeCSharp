using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IMobility
{
    
    public Vector3i Position {get; set;}
    public float Speed {get; set;}
    public float GetCellMovementCost(World.Cell cell);

}