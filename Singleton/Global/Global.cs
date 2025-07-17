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
using ChessLike.World;
using ChessLike.World.Encounter;

public static partial class Global
{
    public static Grid EncounterGrid;
    private static Window RootNode = new Node().GetWindow();
    private static Debug debug = new();
    public static void ConnectToWindow(Window window)
    {
        //Ignore if already done.
        if (window == RootNode)
        {
            return;
        }
        RootNode = window;

        RootNode.WindowInput += GInput.ParseMouseInputAsActionEvent;
        if (RootNode == null) { throw new Exception("No window found."); }

        DebugDisplay.Add(debug);
    }

    static Global()
    {
        Console.WriteLine(Directory.GetContentDir(EDirectory.GAME_CONTENT));
        Console.WriteLine(Directory.GetContentDir(EDirectory.USER_CONTENT));
        SetupManagers();

        EventBus.EncounterLoading += OnEncounterLoading;
    }


    public static Node GetRoot() => RootNode;

    public static SceneTree GetTree() => GetRoot().GetTree();

    public static Grid GetEncounterGrid() => EncounterGrid;

    #region Event Handling
    private static void OnEncounterLoading(EncounterData obj)
    {
        EncounterGrid = obj.Grid;
    }
    #endregion
}
