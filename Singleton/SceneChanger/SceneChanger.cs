using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World.Encounter;
using ChessLike.WorldMap;
using Godot;

namespace ChessLike.Singleton;

public partial class SceneChanger : Node
{
	private static SceneChanger Instance;

	private static Node? CurrentScene;

	public SceneChanger()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		base._Ready();
		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
		EventBus.LoadAttempted += OnLoadAttempted;
		EventBus.MapLocationConfirmed += OnMapLocationConfirmed;
	}

	public Node? GetCurrentScene()
		=> CurrentScene;

	private static Node GetCommonParent()
		=> Instance.GetTree().Root;

	private static void RemoveCurrent()
	{
		CurrentScene?.QueueFree();
		CurrentScene = null;
	}

	public static void ChangeToCombat(EncounterData data)
	{
		RemoveCurrent();

		CombatScene node = Readonly.Scenes.MAIN_COMBAT;

		GetCommonParent().AddChild(node);

		EventBus.SceneChanged?.Invoke(node);

		CurrentScene = node;

		node.Setup(data);
	}

	private static void ChangeToTravelMap()
	{
		RemoveCurrent();

		TravelMapScene node = Readonly.Scenes.MAIN_TRAVEL_MAP;

		GetCommonParent().AddChild(node);

		EventBus.SceneChanged?.Invoke(node);

		CurrentScene = node;
	}

	public static void ChangeToMainMenu()
	{
		RemoveCurrent();

		MainMenuScene node = Readonly.Scenes.MAIN_MENU;

		GetCommonParent().CallDeferred("add_child", node);

		EventBus.SceneChanged?.Invoke(node);

		CurrentScene = node;
	}

	#region Event Handling
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.QUIT) ChangeToMainMenu();
	}

	private void OnLoadAttempted(bool success)
	{
		if (!success) return;

		ChangeToTravelMap();
	}

	private void OnMapLocationConfirmed(TravelMapLocation obj)
	{
		if (obj.CombatEncounter is not null)
			ChangeToCombat(obj.CombatEncounter);
	}
	#endregion
}
