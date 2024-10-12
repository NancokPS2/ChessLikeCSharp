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
		//NodePartyListUI.ButtonPressed += (btn, mob) => NodeEquipmentUI.Update(mob);
	}

	public void Update()
	{
		NodePartyListUI.Update(ChessLike.Entity.EFaction.PLAYER);
		if (NodePartyListUI.MobSelected is not null)
		{
			NodeMobEquipmentUI.Update(NodePartyListUI.MobSelected);
			NodeFactionInventoryUI.Update(Global.ManagerFaction.GetFromEnum(ChessLike.Entity.EFaction.PLAYER));

		}
	}
}
