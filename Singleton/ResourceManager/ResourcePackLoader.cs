using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;

namespace Godot;
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

    public void PrepareDir<TRes>(ResourcePack<TRes> pack) where TRes : Resource
    {
        DirAccess.MakeDirAbsolute(GetBaseDirectory(true) + pack.UNIQUE_STRING);
        DirAccess.MakeDirAbsolute(GetBaseDirectory(false) + pack.UNIQUE_STRING);
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
