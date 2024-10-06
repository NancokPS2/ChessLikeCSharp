using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared;

public interface ISceneDependency
{
    public string SCENE_PATH {get;}
}

public static class ISceneDependencyExtension
{
    public static TNodeType GetInstantiatedScene<TNodeType>(this ISceneDependency @this) where TNodeType : Node
    {
        PackedScene? packed = GD.Load<PackedScene>(@this.SCENE_PATH);

        if (packed is null){throw new Exception("Could not load from path " + @this.SCENE_PATH);}

        TNodeType? instance = packed.Instantiate<TNodeType>();

        return instance;
    }

}
