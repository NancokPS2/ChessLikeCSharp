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
    private static Window RootNode = new Node().GetWindow();
    public static Node2D DrawNode = new(){TopLevel = true, ZIndex = Readonly.LAYER_GLOBAL_DRAW};
    private static Debug debug = new();
    public static void ConnectToWindow(Window window)
    {
        //Ignore if already done.
        if (window == RootNode)
        {
            return;
        }
        RootNode = window;
        
        if (!DrawNode.IsInsideTree())
        {
            CanvasLayer layer = new(){Layer = Readonly.LAYER_GLOBAL_DRAW};
            layer.AddChild(DrawNode);
            RootNode.GetTree().CurrentScene.CallDeferred("add_child", layer);
            DrawNode.Name = "DEBUG_DRAWER";
        }
        //GD.Print(DrawNode.GetPath());

        RootNode.WindowInput += GInput.ParseMouseInputAsActionEvent;
        if (RootNode == null){throw new Exception("No window found.");}

        DebugDisplay.Instance.Add(debug);

    }

    static Global()
    {
        Console.WriteLine(Directory.GetContentDir(EDirectory.GAME_CONTENT));
        Console.WriteLine(Directory.GetContentDir(EDirectory.USER_CONTENT));
        SetupManager();

    }
}



