using ChessLike.Entity;
using Godot;
using System;
using System.Collections;

[GlobalClass]
public partial class PartyGeneralUI : Control, ISceneDependency
{
	private const int DEBUG_ACTION_SAVE_JOBS = 0;
	private const int DEBUG_ACTION_SAVE_UNIT = 1;
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
		NodeDebugOptions.GetPopup().AddItem("SAVE", DEBUG_ACTION_SAVE_JOBS);
		NodeDebugOptions.GetPopup().AddItem("Save Unit", DEBUG_ACTION_SAVE_UNIT);

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
		SaveDialog saver = new SaveDialog(this);
		switch (id)
		{
		case DEBUG_ACTION_SAVE_JOBS:
			List<Resource> job_prototypes = (from job in Global.ManagerJob.CreatePrototypes() select job.ToResource()).ToList<Resource>();
			saver.Use(job_prototypes);
			break;
		
		case DEBUG_ACTION_SAVE_UNIT:
			if (NodePartyListUI is not null && NodePartyListUI.MobSelected is not null)
			{
				saver.Use(NodePartyListUI.MobSelected.ToResource());
				
			} else
			{
				MessageQueue.AddMessage("No unit has been selected, cannot save.");
			}
			break;
		}
	}
}
