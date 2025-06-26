using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;

namespace Godot;

[Obsolete("Unused")]
public partial class ResourcePackLoader : Node
{
    public static ResourcePackLoader Instance { get => instance ?? throw new Exception(); set => instance = value; }
    private static ResourcePackLoader? instance;
    public ResourcePack<ActionEvent> ActionEvents = new();


    public ResourcePackLoader()
    {
        instance = instance is null ? this : instance;

        PrepareDir(ActionEvents);
    }

    public void PrepareDir<TRes>(ResourcePack<TRes> pack) where TRes : Resource, new()
    {
        //DirAccess.MakeDirAbsolute(GetBaseDirectory(true) + GetUniqueString());
        //DirAccess.MakeDirAbsolute(GetBaseDirectory(false) + GetUniqueString());
    }

    public string GetBaseDirectory(bool user)
    {
        if (user)
        {
            return "user://Resources/";
        }
        else
        {
            return "res://Resources/";
        }
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
