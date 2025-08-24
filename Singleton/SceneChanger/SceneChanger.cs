using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.WorldMap;
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

	public static async void ChangeToCombat(EncounterData data)
	{
		LoadingScreen.SetLoading(ELoadingReason.CHANGING_SCENE, true);
		RemoveCurrent();

		CombatScene node = Readonly.Scenes.MAIN_COMBAT;

		await AsyncSceneChange(node);

		node.Setup(data);
		
	}

	private static async void ChangeToTravelMap()
	{
		LoadingScreen.SetLoading(ELoadingReason.CHANGING_SCENE, true);
		RemoveCurrent();

		TravelMapScene node = Readonly.Scenes.MAIN_TRAVEL_MAP;

		await AsyncSceneChange(node);
		LoadingScreen.SetLoading(ELoadingReason.CHANGING_SCENE, false);
	}


	public static async void ChangeToMainMenu()
	{
		LoadingScreen.SetLoading(ELoadingReason.CHANGING_SCENE, true);
		RemoveCurrent();

		MainMenuScene node = Readonly.Scenes.MAIN_MENU;

		await AsyncSceneChange(node);
		LoadingScreen.SetLoading(ELoadingReason.CHANGING_SCENE, false);
	}

	private static async Task AsyncSceneChange<TNode>(TNode node) where TNode : Node
	{
		await Task.Run(
			async () =>
			{
				float time = Time.GetTicksMsec();
				new Callable(GetCommonParent(), "add_child").CallDeferred(node);
				await Instance.ToSignal(node, Node.SignalName.Ready);
				MsgLog.LogInfoMsg($"Finished async {typeof(TNode).ToString().GetExtension()} load. Time passed: {(Time.GetTicksMsec() - time) / 1000}");

				EventBus.SceneChanged?.Invoke(node);

				CurrentScene = node;
			}
		);
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
