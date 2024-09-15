global using System.Collections.Generic;
global using System.Linq;
global using System.IO;
global using System.Numerics;
global using System.Drawing;

global using ChessLike.Shared;
global using ChessLike.Shared.Identification;
global using ChessLike.Shared.GenericStruct;
global using ChessLike;

using ChessLike.Entity;
using Action = ChessLike.Entity.Action;
using Godot;

public static partial class Global
{
    public static void Setup()
    {

    }

    static Global()
    {
        Console.WriteLine(Directory.GetContentDir(EDirectory.GAME_CONTENT));
        Console.WriteLine(Directory.GetContentDir(EDirectory.USER_CONTENT));
        SetupManager();

    }
}



