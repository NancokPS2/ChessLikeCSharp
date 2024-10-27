using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;


namespace ChessLike.Shared;

public interface IVisual3D
{
    public MeshInstance3D GetMeshInstance();
}
