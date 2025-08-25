using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	const string META_BACKGROUND_NODE = "UIManagerBACKGROUND_NODE";
	const string META_CAN_BE_BACKED_OUT = "UIManagerCAN_BE_BACKED_OUT";
	public static UIManager Instance = null!;

	[Export]
	protected PackedScene BackgroundScene = null!;

	[Export]
	protected Godot.Collections.Dictionary<EUIScene, PackedScene> Scenes = new();
	[Export]
	protected CanvasLayer CanvasLayerNode = null!;
	protected static Dictionary<EUIScene, Node> Nodes = new();

	protected static List<(EUIScene, Node)> ActiveNodes = new();

	protected static Dictionary<ELayer, CanvasWithTarget> CanvasLayers = new();

	public override void _Ready()
	{
		base._Ready();
		Instance = this;

		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
		EventBus.CombatStateChanged += OnCombatStateChanged;
		EventBus.SceneChanged += OnSceneChange;
		EventBus.InputPause += OnInputPause;

		foreach (var item in Scenes)
		{
			Node node = item.Value.Instantiate();
			Nodes[item.Key] = node;
			ActivateUI(item.Key, new());
		}
		ChangeToUI(EUIScene.NONE, new());
	}

	public static CanvasWithTarget GetLayer(ELayer layer)
	{
		if (!CanvasLayers.ContainsKey(layer))
		{
			CanvasLayers[layer] = new() { Layer = (int)layer, Name = layer.ToString() };
			Instance.AddChild(CanvasLayers[layer]);
		}

		CanvasWithTarget canvas = CanvasLayers[layer];

		return canvas;
	}

	public static Node2D GetLayerDrawTarget(ELayer layer) => GetLayer(layer).DrawTarget;

	public static int GetLayerCount() => CanvasLayers.Values.Count;
	/* 
		public static IPopup<TEnum> ShowEnum<TEnum>()
		{
			IPopup popup = new();
			AddToParent(GetLayer(ELayer.POPUP));
		}
	 */
	public static List<CanvasWithTarget> GetCanvasLayers() => CanvasLayers.Values.ToList();

	public string GetText()
	{
		return CanvasLayers.ToStringList();
	}

	public static Node GetUINode(EUIScene ui)
	{
		Node found = Nodes[ui];
		return found;
	}

	public static bool IsUINodeActive(EUIScene ui)
		=> ActiveNodes.Contains((ui, GetUINode(ui)));

	public static void ActivateUI(EUIScene ui)
		=> ActivateUI(ui, new());

	protected static void ActivateUI(EUIScene ui, UISceneParameters parameters)
	{
		if (ui == EUIScene.KEEP || ui == EUIScene.KEEP)
		{
			MsgLog.LogErrorMsg($"{ui} is not a valid mode for UIManager.AddUI()");
			return;
		}

		(EUIScene, Node) tuple = (ui, GetUINode(ui));

		//Verify everything is ok before activating.
		bool isActive = ActiveNodes.Contains(tuple);
		bool isInsideTree = tuple.Item2.IsInsideTree();
		if (isActive || isInsideTree)
		{
			MsgLog.LogErrorMsg($"Tried to add {ui} UI but... already in the tree?{isInsideTree} | already active?{isActive}. Aborting.");
			return;
		}

		//Activate it
		Instance.CanvasLayerNode?.AddChild(tuple.Item2);
		ActiveNodes.Add(tuple);

		//Add a background if applicable.
		if (parameters.NeedsBackground)
			AddBackground(tuple.Item2);

		//Set if you can or can't back out of it with DeactivateLastUI().
		tuple.Item2.SetMeta(META_CAN_BE_BACKED_OUT, parameters.CanBeBackedOut);

		CleanBackgrounds();
	}

	protected static void DeactivateLastUI()
	{
		List<(EUIScene, Node)> candidates = new(ActiveNodes
			.Where(x => NodeCanBeBackedOut(x.Item2))
			);

		if (candidates.IsEmpty())
			MsgLog.LogErrorMsg($"Tried to deactivate the last UI, but there was none active.");
		else
			DeactivateUI(candidates.Last().Item1);
	}

	protected static void AddBackground(Node node)
	{
		if (node.GetParent() != Instance.CanvasLayerNode) throw new Exception();

		Control background = Instance.BackgroundScene.Instantiate<Control>();
		Instance.CanvasLayerNode.AddChild(background);

		int nodeIndex = node.GetIndex();

		Instance.CanvasLayerNode.MoveChild(background, nodeIndex);

		background.SetAnchorsPreset(LayoutPreset.FullRect);
		background.SetMeta(META_BACKGROUND_NODE, true);
	}

	protected static void CleanBackgrounds()
	{
		List<Node> children = new(Instance.CanvasLayerNode.GetChildren());

		//Nothing to clean.
		if (children.IsEmpty()) return;

		//If there's only backgrounds left, just remove them all.
		if (children.All(x => NodeIsBackground(x)))
		{
			children.ForEach(x => x.QueueFree());
			return;
		}

		//If a background is on top, remove it.
		if (NodeIsBackground(children.Last()))
			children.Last().QueueFree();

		//Make sure all is in order.
		Node? previousNode = null;
		List<Node> toFree = new();
		foreach (var node in children)
		{
			//Two backgrounds in a row, remove one.
			if (NodeIsBackground(node) && previousNode is not null && NodeIsBackground(previousNode))
				toFree.Add(node);

			previousNode = node;
		}

		toFree.ForEach(x => x.QueueFree());
	}

	protected static bool NodeIsBackground(Node node)
		=> node.GetMeta(META_BACKGROUND_NODE, false).As<bool>();

	protected static bool NodeCanBeBackedOut(Node node)
		=> node.GetMeta(META_CAN_BE_BACKED_OUT, true).As<bool>();

	protected static void DeactivateUI(EUIScene ui)
	{
		if (ui == EUIScene.KEEP || ui == EUIScene.KEEP)
		{
			MsgLog.LogErrorMsg($"{ui} is not a valid mode for UIManager.AddUI()");
			return;
		}

		(EUIScene, Node) tuple = (ui, GetUINode(ui));

		//Verify everything is ok before deactivating.
		bool nodeIsInTree = tuple.Item2.IsInsideTree();
		bool nodeIsActive = ActiveNodes.Contains(tuple);
		if (!nodeIsInTree || !nodeIsActive)
			MsgLog.LogErrorMsg($"Deactivating {ui} but... already outside the tree?{!nodeIsInTree} | already inactive?{!nodeIsActive}.");

		//Activate it
		ActiveNodes.Remove(tuple);
		tuple.Item2.RemoveSelf();

		CleanBackgrounds();
	}

	protected static void DeactivateUI()
	=> new List<(EUIScene, Node)>(ActiveNodes).ForEach(x => DeactivateUI(x.Item1));


	public static void ChangeToUI(EUIScene ui)
		=> ChangeToUI(ui, new());

	public static void ChangeToUI(EUIScene ui, UISceneParameters parameters)
	{
		//Do nothing.
		if (ui == EUIScene.KEEP) return;

		DeactivateUI();

		//All UI elements where deactivated, if ui is NONE, end here.
		if (ui == EUIScene.NONE) return;

		ActivateUI(ui, parameters);
	}

	#region Event Handling
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.PARTY) ActivateUI(EUIScene.PARTY, new());
	}

	private void OnCombatStateChanged(ECombatState obj)
	{
		EUIScene eUI = obj switch
		{
			ECombatState.PREPARATION => EUIScene.PREPARATION,
			ECombatState.ACTION_INPUT => EUIScene.COMBAT_GENERAL,
			_ => EUIScene.KEEP,
		};

		ChangeToUI(eUI, new(){CanBeBackedOut = false, NeedsBackground = false});
	}

	private void OnSceneChange(Node node)
	{
		ChangeToUI(EUIScene.NONE, new());
	}

	private void OnInputPause()
	{
		//If there are not other active nodes or the current top node can't be backed out of, open the pause menu.
		if (ActiveNodes.IsEmpty() || !NodeCanBeBackedOut(ActiveNodes.Last().Item2))
			ActivateUI(EUIScene.PAUSE, new());
		else
			DeactivateLastUI();
	}
	#endregion
}
