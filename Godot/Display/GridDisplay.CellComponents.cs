using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace Godot;

public partial class GridDisplay : Node3D
{
    public class CellComponents
    {
        public MeshInstance3D mesh_instance;
        public Vector3i position;

        public CellComponents(Cell cell, Vector3i position)
        {
            mesh_instance = new MeshInstance3D();

            if (!cell.flags.Contains(Cell.Flag.AIR))
            {
                mesh_instance.Mesh = new BoxMesh();
            }

            this.position = position;
            mesh_instance.Position = (Vector3)position.ToGVector3();
        }
    }
}
