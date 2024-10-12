using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using Godot.Display;
using System;

[GlobalClass]
public partial class CombatGeneralUI : Control, ISceneDependency
{
	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Combat/CombatGeneralUI.tscn";

	[Export]
	public CombatTurnUI? NodeTurnUI;
	[Export]
	public CombatActionUI? NodeActionUI;
	[Export]
	public CombatConfirmationUI? NodeConfirmationUI;
	[Export]
	public MobGeneralUI? NodeMobUI;

	private BattleController? BattleControllerCurrent;
	
	public override void _Ready()
	{
		NodeTurnUI ??= (CombatTurnUI?)FindChild("CombatTurnUI");
		NodeActionUI ??= (CombatActionUI?)FindChild("CombatActionUI");
		NodeConfirmationUI ??= (CombatConfirmationUI)FindChild("CombatConfirmationUI");
		NodeMobUI ??= (MobGeneralUI)FindChild("MobGeneralUI");
	}

	public void Update(BattleController controller)
	{
		BattleControllerCurrent = controller;
	
		if (controller is null) {throw new Exception("Null controller!???");}

		Mob taking_turn = controller.CompTurnManager.GetCurrentTurnTaker() as Mob;

		NodeTurnUI.Update(controller.CompTurnManager);
		NodeActionUI.Update(taking_turn);
		NodeMobUI.Update(taking_turn);
	}

}
