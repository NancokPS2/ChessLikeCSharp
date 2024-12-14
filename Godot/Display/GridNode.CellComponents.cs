using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace Godot;

public partial class GridNode : Node3D
{
    public class CellComponent
    {
        public Dictionary<Layer, MeshInstance3D> mesh_instances = new();
        public StaticBody3D collision_body = new(){InputRayPickable = true};
        public CollisionShape3D collision_shape = new(){Shape = new BoxShape3D()};

        public CellComponent(Grid.Cell cell)
        {
            mesh_instances.Add(Layer.BASE, new MeshInstance3D());

            if (cell.flags.Contains(ECellFlag.SOLID))
            {
                mesh_instances[Layer.BASE].Mesh = Global.Resources.GetMesh(Global.Resources.MeshIdent.CELL_FULL);
            }
        }
    }
}
