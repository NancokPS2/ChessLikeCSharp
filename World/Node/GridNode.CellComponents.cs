using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace Godot;

public partial class GridNode : Node3D
{
    protected class CellComponent
    {
        public Dictionary<Layer, MeshInstance3D> MeshInstances = new();
        public StaticBody3D CollisionBody = new() { InputRayPickable = true };
        public CollisionShape3D CollisionShape = new() { Shape = new BoxShape3D() };

        public CellComponent(GridCell cell)
        {
            MeshInstances.Add(Layer.BASE, new MeshInstance3D());

            if (cell.Flags.Contains(ECellFlag.SOLID))
            {
                MeshInstances[Layer.BASE].Mesh = Global.Resources.GetMesh(Global.Resources.MeshIdent.CELL_FULL);
            }
        }
        
    }
}
