using ChessLike.Entity;
using Godot;
using System;

[GlobalClass]
public partial class PartyGeneralUI : Control, ISceneDependency
{
	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyGeneralUI.tscn";

	[Export]
	public PartyMobListUI? NodePartyListUI;
	[Export]
	public InventoryUI? NodeMobEquipmentUI;
	[Export]
	public InventoryUI? NodeFactionInventoryUI;

	public override void _Ready()
	{
		base._Ready();
		//When a button from the party UI is pressed, update the equipment UI.
		NodePartyListUI.ButtonPressed += OnPartyListUIPressed;
	}

	public void Update()
	{
		NodePartyListUI.Update(ChessLike.Entity.EFaction.PLAYER);
		NodeFactionInventoryUI.Update(Global.ManagerFaction.GetFromEnum(ChessLike.Entity.EFaction.PLAYER));
		if (NodePartyListUI.MobSelected is not null)
		{
			NodeMobEquipmentUI.Update(NodePartyListUI.MobSelected);
		}
	}

	public void OnPartyListUIPressed(Button button, Mob mob)
	{
		NodeMobEquipmentUI.Update(mob);
	}
}
