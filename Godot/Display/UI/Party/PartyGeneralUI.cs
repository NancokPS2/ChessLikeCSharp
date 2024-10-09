using Godot;
using System;

[GlobalClass]
public partial class PartyGeneralUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyGeneralUI.tscn";

    [Export]
    public PartyMobListUI? NodePartyListUI;
    [Export]
    public PartyMobEquipmentUI? NodeEquipmentUI;

	public void Update()
	{
        NodePartyListUI.Update(ChessLike.Entity.EFaction.PLAYER);
        if (NodePartyListUI.MobSelected is not null)
        {
            NodeEquipmentUI.Update(NodePartyListUI.MobSelected);
        }
	}
}
