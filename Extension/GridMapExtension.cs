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

    public static void SetItemMeta(this MeshLibrary @this, int id, Variant variant)
    {
        @this.SetMeta($"ItemMeta{id}", variant);
    }
    public static Variant GetItemMeta(this MeshLibrary @this, int id, Variant def = default)
    {
        return @this.GetMeta($"ItemMeta{id}", def);
    }
    public static bool HasItemMeta(this MeshLibrary @this, int id)
    {
        return @this.HasMeta($"ItemMeta{id}");
    }
    public static int FindIdWithMeta<[MustBeVariant] TMeta>(this MeshLibrary @this, TMeta meta) where TMeta : IEquatable<TMeta>
    {
        foreach (var id in @this.GetItemList())
        {
            if (!@this.HasItemMeta(id)) continue;
            if (@this.GetItemMeta(id).As<TMeta>().Equals(meta)) return id;
        }
        return -1;
    }
}
