using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace Godot;

public partial class GridDisplay : Node3D
{
    public class CellComponent
    {
        public Dictionary<Layer, MeshInstance3D> mesh_instances = new();


        public CellComponent(Grid.Cell cell)
        {
            mesh_instances.Add(Layer.BASE, new MeshInstance3D());

            if (!cell.flags.Contains(CellFlag.AIR))
            {
                mesh_instances[Layer.BASE].Mesh = new BoxMesh();
            }
        }
    }
}
