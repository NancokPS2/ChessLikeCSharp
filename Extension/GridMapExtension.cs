using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Extension;

public static class GridMapExtension
{


    public static Vector3i GetDimensions(this GridMap @this)
    {
        Godot.Collections.Array<Vector3I> cells = @this.GetUsedCells();
        Vector3i output = new();
        foreach (var item in cells)
        {
            output.X = Mathf.Max(output.X, item.X);
            output.Y = Mathf.Max(output.Y, item.Y);
            output.Z = Mathf.Max(output.Z, item.Z);
        }
        return output;
    }
}
