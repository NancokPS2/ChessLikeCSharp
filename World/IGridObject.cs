using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public interface IGridObject
{
    public Vector3i GetPosition();
    public bool IsValidMove(Grid grid, Vector3i from, Vector3i to);
    public bool IsValidPositionToExist(Grid grid, Vector3i position);
    public int PathingGetHorizontalRange();
    public int PathingGetVerticalRange();
}
