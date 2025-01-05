using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;

public partial class UI: Node2D, IDebugDisplay
{
    public enum ELayer {
        BASE_LAYER,
        PAUSE_MENU,
        MSG_QUEUE,
        GLOBAL_DRAW,
        CHEAT_INPUT,
    }

    public static UI Instance;

    private Dictionary<ELayer, CanvasLayer> CanvasLayers = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public static CanvasLayer GetLayer(ELayer layer)
    {
        if (!Instance.CanvasLayers.ContainsKey(layer))
        {
            Instance.CanvasLayers[layer] = new(){Layer = (int)layer};
        }

        CanvasLayer canvas = Instance.CanvasLayers[layer];
        Instance.AddChild(canvas);

        return canvas;

    }

    public static int GetLayerCount() => Instance.CanvasLayers.Values.Count;

    public static List<CanvasLayer> GetCanvasLayers => Instance.CanvasLayers.Values.ToList();

    public string GetText()
    {
        return CanvasLayers.ToStringList();
    }
}
