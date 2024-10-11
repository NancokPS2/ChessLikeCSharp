using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobGeneralUI : Control, ISceneDependency
{
	[Export]
	public TabContainer? NodeTabContainer;
	[Export]
	public MobEquipmentUI? NodeEquipmentUI;
	[Export]
	public MobActionUI? NodeActionUI;

	[Export]
	public MobStatsUI? NodeStatsUI;

    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobGeneralUI.tscn";
    
	public override void _Ready()
	{
		NodeEquipmentUI ??= (MobEquipmentUI?)FindChild("Equipment");
		NodeStatsUI ??= (MobStatsUI?)FindChild("Stats");
		NodeActionUI ??= (MobActionUI?)FindChild("Action");
		NodeTabContainer ??= this.GetChild<TabContainer>();

		NodeTabContainer.TabChanged += (x) => Update(MobCurrent);
	}

	private Mob? MobCurrent;

	public void Update(Mob mob)
	{
		if(NodeTabContainer is null) {throw new Exception("Null TabContainer");}

		MobCurrent = mob;

		if (mob is null) {return;}

		Node current_ui = NodeTabContainer.GetChild<Control>(NodeTabContainer.CurrentTab);

		if (current_ui is MobEquipmentUI equip)
		{
			equip.Update(MobCurrent);
		}
		else if (current_ui is MobStatsUI stats)
		{
			stats.Update(mob);
		}
		else if (current_ui is MobActionUI action)
		{
			action.Update(mob);
		}
		
	}

    private void DisplayDummy()
    {
        Mob mob = Mob.CreatePrototype(EMobPrototype.HUMAN);
        mob.MobInventory.AddItem(new Trinket());
        Update(mob);
    }

}
