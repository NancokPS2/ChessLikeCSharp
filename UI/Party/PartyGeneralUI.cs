
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
	public MobListUI? NodePartyListUI;

	[Export]
	public MenuButton? NodeDebugOptions;

	[Export]
	public bool DebugButtons = true;

	public override void _Ready()
	{
		base._Ready();

		NodeDebugOptions.GetPopup().IdPressed += OnIdPressed;
		NodeDebugOptions.GetPopup().AddItem("SAVE", DEBUG_ACTION_SAVE_JOBS);
		NodeDebugOptions.GetPopup().AddItem("Save Unit", DEBUG_ACTION_SAVE_UNIT);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		Update();
	}


	[Obsolete("Remove dependencies to pause menu, then delete.")]
	public void Update()
	{
	}
	private void OnIdPressed(long id)
	{	
		SaveDialog saver = new SaveDialog(this);
		switch (id)
		{

			case DEBUG_ACTION_SAVE_UNIT:
				if (NodePartyListUI is not null && NodePartyListUI.MobSelected is not null)
				{
					saver.Use(NodePartyListUI.MobSelected);

				}
				else
				{
					MessageQueue.AddMessage("No unit has been selected, cannot save.");
				}
				break;
		default: throw new Exception($"Invalid value {id}");
		}
	}
}
