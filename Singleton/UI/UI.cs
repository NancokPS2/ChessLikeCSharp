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

    public static UI Instance;

    private Dictionary<ELayer, CanvasWithTarget> CanvasLayers = new();

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public static CanvasWithTarget GetLayer(ELayer layer)
    {
        if (!Instance.CanvasLayers.ContainsKey(layer))
        {
            Instance.CanvasLayers[layer] = new(){Layer = (int)layer, Name = layer.ToString()};
            Instance.AddChild(Instance.CanvasLayers[layer]);
        }

        CanvasWithTarget canvas = Instance.CanvasLayers[layer];

        return canvas;

    }

    public static Node2D GetLayerDrawTarget(ELayer layer) => GetLayer(layer).DrawTarget;

    public static int GetLayerCount() => Instance.CanvasLayers.Values.Count;
/* 
    public static IPopup<TEnum> ShowEnum<TEnum>()
    {
        IPopup popup = new();
        AddToParent(GetLayer(ELayer.POPUP));
    }
 */
    public static List<CanvasWithTarget> GetCanvasLayers => Instance.CanvasLayers.Values.ToList();

    public string GetText()
    {
        return CanvasLayers.ToStringList();
    }
}
