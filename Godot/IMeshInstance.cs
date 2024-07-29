using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Godot;

/// <summary>
/// Can be used to display a MeshInstance3D
/// </summary>
public interface IMeshInstance
{
    public Vector3 GetMeshPosition();
}
