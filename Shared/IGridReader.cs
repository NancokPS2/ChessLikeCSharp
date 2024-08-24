using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Shared;

public interface IGridReader
{
    public Grid.Cell? GetCell(Grid grid)
    {
        if(this is IGridPosition gridPosition)
        {
            return GetCell(grid, gridPosition.Position);
        } else
        {
            return null;
        }
    }

    public Grid.Cell GetCell(Grid grid, Vector3i position)
    {
        return grid.GetCell(position);
    }
    public bool IsFlagAtPosition(Grid grid, Vector3i position, CellFlag flag)
    {
        return grid.IsFlagInPosition(position, flag);
    }
}
