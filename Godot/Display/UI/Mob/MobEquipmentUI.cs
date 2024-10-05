using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    public ItemList<Inventory.Slot>? EquipList;

    [Export]
	public Control? NodeList;

    public override void _Ready()
    {
        base._Ready();
		NodeList ??= (Control)FindChild("EQUIPMENT");

        EquipList = new(NodeList);
    }

    public void Update(Mob mob)
    {
        if(EquipList is null) {throw new Exception("Null ActionList");}

        EquipList.ClearItems();
        GD.Print(mob.Inventory.GetSlots().ToArray());
        foreach (var slot in mob.Inventory.GetSlots())
        {
            Button panel = new()
            {
                AnchorRight = 1.0f,
                FocusMode = Control.FocusModeEnum.All,
            };
            Label label = new()
            {
                AnchorRight = 1,
                AnchorBottom = 1,
                OffsetLeft = panel.Scale.Y + 4,
                LabelSettings = new(),
            };
            TextureRect textureRect = new()
            {
                AnchorBottom = 1,
                CustomMinimumSize = new(panel.Scale.Y, panel.Scale.Y),
            };
            var menu_item = new ItemList<Inventory.Slot>.MenuItem().ChainSetContained(slot);

            if(slot.Item is not null) {menu_item.ChainSetText(slot.Item.Name);}
            else if (slot.FlagWhitelist.Count != 0) {menu_item.ChainSetText(slot.FlagWhitelist[0].ToString());}

            EquipList.AddItem( menu_item );
        }
    }
}
