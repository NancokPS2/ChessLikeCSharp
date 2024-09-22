using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Extension;

public static class PackedSceneExtension
{
    public static Mesh GetMeshFromModel(this PackedScene @this)
    {
        Node root = @this.Instantiate();
        if (root is Node3D node)
        {
            if (node.GetChild(0) is MeshInstance3D mesh_inst)
            {
                return mesh_inst.Mesh;
            }
        }

        throw new Exception("This is not a model");
    }
}
