using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Godot;

public class SceneResourcePack<TNode> : ResourcePack<PackedScene> where TNode : Node3D
{
    public SceneResourcePack() : base()
    {
    }

    public SceneResourcePack(string uniqueString) : base(uniqueString)
    {
    }

    public TNode GetInstance(string identifier)
        => GetResource(identifier).Instantiate<TNode>();
}
