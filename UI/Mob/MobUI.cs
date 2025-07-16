using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobUI : Control, ISceneDependency
{
	[Export]
	public TabContainer? NodeTabContainer;
	[Export]
	public MobEquipmentUI? NodeEquipmentUI;
	[Export]
	public MobActionUI? NodeActionUI;

	[Export]
	public MobStatsUI? NodeStatsUI;

	[Export]
	public Label? NodeNameUI;

	public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobGeneralUI.tscn";

	private Mob? MobCurrent;

	public MobUI()
	{
	}

	public override void _Ready()
	{
		base._Ready();
		EventBus.MobSelected += OnMobSelected;

		NodeTabContainer.TabChanged += (x) => Update(MobCurrent);
		DisplayDummy();
	}

	public void Update(Mob mob)
	{
		if (NodeTabContainer is null) { throw new Exception("Null TabContainer"); }
		if (mob is null) { return; }

		MobCurrent = mob;

		NodeNameUI.Text = mob.DisplayedName;		
	}

	private void DisplayDummy()
	{
		Mob mob = Mob.CreatePrototype(EMobPrototype.HUMAN);
		Update(mob);
	}

	#region Event Handling
	private void OnMobSelected(Mob obj)
    {
		Update(obj);
    }
	#endregion

}
