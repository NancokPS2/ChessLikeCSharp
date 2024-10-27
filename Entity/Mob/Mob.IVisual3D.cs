using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

public partial class Mob : IVisual3D
{
    private MeshInstance3D _mesh_inst = new(){Mesh = new SphereMesh()};
    public MeshInstance3D GetMeshInstance()
    {
        return _mesh_inst;
    }
}
