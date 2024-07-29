using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.World;

public class Filter
{
    //public static Func<Vector3i, bool> NONE = (x => true);
    public Grid grid;

    public Filter(Grid _grid)
    {
        this.grid = _grid;
    }

    public bool None(Vector3i cell)
    {
        return true;
    }
    public bool SolidBelow(Vector3i cell)
    {
        Cell below = grid.GetCell(cell + Vector3i.DOWN);
        return below.flags.Contains(Cell.Flag.SOLID);
    }
}