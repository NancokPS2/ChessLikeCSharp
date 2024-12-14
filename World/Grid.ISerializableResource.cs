using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.World;

public partial class Grid : IResourceSerialize<Grid, GridResource>
{
    public GridResource ToResource()
    {
        GridResource output = new();
        output.Boundary = Boundary.ToGVector3I();
        foreach (var item in CellDictionary)
        {
            Vector3I pos = item.Key.ToGVector3I();
            output.CellNames[pos] = item.Value.Name;
            output.CellFlags[pos] = new(item.Value.Flags);
            output.CellSelectables[pos] = item.Value.Selectable;
        }
        return output;
    }

    public static Grid FromResource(GridResource res)
    {
        if (res.CellNames.Count != res.CellFlags.Count || res.CellNames.Count != res.CellSelectables.Count)
        {
            throw new Exception();
        }
        Grid output = new();

        output.Boundary = new(res.Boundary);
        foreach (var item in res.CellNames.Keys)
        {
            Cell new_cell = new(
                res.CellNames[item],
                new(res.CellFlags[item]),
                res.CellSelectables[item]
            );
        }
        return output;
    }
}
