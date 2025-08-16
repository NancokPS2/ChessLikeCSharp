using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;

[GlobalClass]
public partial class UIManager : Control, IDebugDisplay
{
	public static UIManager Instance;

	[Export]
	protected Godot.Collections.Dictionary<EUIScene, PackedScene> Scenes = new();
	[Export]
	protected CanvasLayer? CanvasLayerNode;
	protected Godot.Collections.Dictionary<EUIScene, Node> Nodes = new();

	private Dictionary<ELayer, CanvasWithTarget> CanvasLayers = new();

	public override void _Ready()
	{
		base._Ready();
		Instance = this;

		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
		EventBus.CombatStateChanged += OnCombatStateChanged;
		EventBus.SceneChanged += OnSceneChange;

		foreach (var item in Scenes)
		{
			Node node = item.Value.Instantiate();
			Nodes[item.Key] = node;
			CanvasLayerNode.AddChild(node);
		}
		ChangeToUI(EUIScene.NONE);
	}

	public static CanvasWithTarget GetLayer(ELayer layer)
	{
		if (!Instance.CanvasLayers.ContainsKey(layer))
		{
			Instance.CanvasLayers[layer] = new() { Layer = (int)layer, Name = layer.ToString() };
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

	public EUIScene GetCurrentUI()
		=> Nodes.Keys.First(x => Nodes[x].IsInsideTree());

	public static void ChangeToUI(EUIScene ui)
	{
		//Do nothing.
		if (ui == EUIScene.KEEP) return;

		foreach (var item in Instance.Nodes.Values.Where( x => x.IsInsideTree()))
		{
			Instance.CanvasLayerNode.RemoveChild(item);
		}

		//Just hide all UI elements.
		if (ui == EUIScene.NONE) return;

		Node node = Instance.Nodes[ui];
		Instance.CanvasLayerNode.AddChild(node);
	}

	#region Event Handling
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.PARTY) ChangeToUI(EUIScene.PARTY);
	}

	private void OnCombatStateChanged(ECombatState obj)
	{
		EUIScene eUI = obj switch
		{
			ECombatState.PAUSED => EUIScene.PAUSE,
			ECombatState.PREPARATION => EUIScene.PREPARATION,
			ECombatState.ACTION_INPUT => EUIScene.COMBAT_GENERAL,
			_ => EUIScene.KEEP,
		};

		ChangeToUI(eUI);
	}

	private void OnSceneChange(Node node)
	{
		ChangeToUI(EUIScene.NONE);
	}
	#endregion
}
