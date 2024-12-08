using ChessLike.Entity;
using Godot;
using System;

[GlobalClass]
public partial class PartyGeneralUI : Control, ISceneDependency
{
	private const int DEBUG_ACTION_SAVE = 0;
	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Party/PartyGeneralUI.tscn";

	[Export]
	public PartyMobListUI? NodePartyListUI;
	[Export]
	public InventoryUI? NodeMobEquipmentUI;
	[Export]
	public InventoryUI? NodeFactionInventoryUI;
	[Export]
	public PartyJobChangeUI? NodePartyJobChangeUI;

	[Export]
	public MenuButton? NodeDebugOptions;

	[Export]
	public bool DebugButtons = true;

	public override void _Ready()
	{
		base._Ready();
		//When a button from the party UI is pressed, update the equipment UI.
		NodePartyListUI.ButtonPressed += OnPartyListUIPressed;

		NodeDebugOptions.GetPopup().IdPressed += OnIdPressed;
		NodeDebugOptions.GetPopup().AddItem("SAVE", DEBUG_ACTION_SAVE);

	}

	public void Update()
	{
		NodePartyListUI.Update(ChessLike.Entity.EFaction.PLAYER);
		NodeFactionInventoryUI.Update(Global.ManagerFaction.GetFromEnum(ChessLike.Entity.EFaction.PLAYER));
	}

	public void OnPartyListUIPressed(Button button, Mob mob)
	{
		NodeMobEquipmentUI.Update(mob);
		NodePartyJobChangeUI.Update(mob);
	}
	private void OnIdPressed(long id)
	{
		if (id == DEBUG_ACTION_SAVE)
		{
			SaveDialog saver = new SaveDialog(this, Job.CreatePrototype(EJob.CIVILIAN).ToResource());
			saver.Use(Job.CreatePrototype(EJob.CIVILIAN).ToResource());
		}
	}
}
