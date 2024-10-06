using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Extension;

public static class BuiltInExtension
{
    public static Godot.Collections.Array ToGArray<TCont>(this List<TCont> @this) where TCont : notnull, Enum
    {
        Godot.Collections.Array output = new();
        output.AddRange(@this);
        return output;
    }
    
}
