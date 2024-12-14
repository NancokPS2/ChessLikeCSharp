using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.World;

public partial class GridResource : Resource
{
    [Export]
    public Godot.Vector3I Boundary = new(10,10,10);
    [Export]
    public Godot.Collections.Dictionary<Vector3I, string> CellNames = new();
    [Export]
    public Godot.Collections.Dictionary<Vector3I, Godot.Collections.Array<ECellFlag>> CellFlags = new();
    [Export]
    public Godot.Collections.Dictionary<Vector3I, bool> CellSelectables = new();
}
