using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Shared;

public interface IGridReader
{
    public Cell? GetCell(Grid grid)
    {
        if(this is IGridPosition gridPosition)
        {
            return GetCell(grid, gridPosition.Position);
        } else
        {
            return null;
        }
    }

    public Cell GetCell(Grid grid, Vector3i position)
    {
        return grid.GetCell(position);
    }
    public bool IsFlagAtPosition(Grid grid, Vector3i position, Cell.Flag flag)
    {
        return grid.IsFlagInPosition(position, flag);
    }
}
