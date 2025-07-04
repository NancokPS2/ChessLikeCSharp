using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using Godot.Display;
using System;

[GlobalClass]
public partial class CombatUI : Control, ISceneDependency
{
	public string SCENE_PATH { get; } = "res://UI/Combat/CombatUI.tscn";

	[Export]
	public CombatTurnUI? NodeTurnUI;
	[Export]
	public CombatActionUI? NodeActionUI;
	[Export]
	public CombatConfirmationUI? NodeConfirmationUI;
	[Export]
	public MobUI? NodeMobUI;

	private BattleController? BattleControllerCurrent;
	
	public override void _Ready()
	{
		NodeTurnUI ??= (CombatTurnUI?)FindChild("CombatTurnUI");
		NodeActionUI ??= (CombatActionUI?)FindChild("CombatActionUI");
		NodeConfirmationUI ??= (CombatConfirmationUI)FindChild("CombatConfirmationUI");
		NodeMobUI ??= (MobUI)FindChild("MobGeneralUI");
	}

}
