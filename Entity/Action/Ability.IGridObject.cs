using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Entity.Action;

public partial class Ability// : IGridObject
{
    public Vector3i GetPosition()
    {
        return Owner.GetPosition();
    }

    public bool IsValidMove(Grid grid, Vector3i from, Vector3i to)
    {
        return true;
    }

    public bool IsValidPositionToExist(Grid grid, Vector3i position)
    {
        return false;
    }

    public int PathingGetHorizontalRange()
    {
        throw new NotImplementedException();
    }

    public int PathingGetVerticalRange()
    {
        throw new NotImplementedException();
    }

    public bool PathingIsInRange(Grid grid, Vector3i position)
    {
        throw new NotImplementedException();
    }
}
