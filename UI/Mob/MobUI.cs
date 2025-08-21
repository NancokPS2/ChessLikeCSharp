using ChessLike.Entity;
using ChessLike.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobUI : BaseMobUI, ISceneDependency
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

	public override void _Ready()
	{
		base._Ready();
		NodeTabContainer.TabChanged += (x) => Update(MobCurrent);
		DisplayDummy();
	}

	protected override void Update(Mob mob)
	{
		base.Update(mob);

		if (NodeTabContainer is null) { throw new Exception("Null TabContainer"); }
		if (mob is null) { return; }

		MobCurrent = mob;

		NodeNameUI.Text = mob.DisplayedName;		
	}

	private void DisplayDummy()
	{
		Mob mob = Mob.GetDefault();
		Update(mob);
	}
}
