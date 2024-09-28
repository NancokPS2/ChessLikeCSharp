using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace ChessLike;

public static class ImmediateMeshExtension
{
    public static void DrawLine(this ImmediateMesh @this, Vector3 from_global, Vector3 to_global)
    {
        @this.SurfaceSetNormal(Vector3.Up);
        @this.SurfaceSetUV(Vector2.One);
        @this.SurfaceAddVertex(from_global);

        @this.SurfaceSetNormal(Vector3.Up);
        @this.SurfaceSetUV(Vector2.One);
        @this.SurfaceAddVertex(to_global);
    }
}
