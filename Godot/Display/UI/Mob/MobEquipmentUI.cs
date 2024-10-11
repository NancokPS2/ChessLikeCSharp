using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : BaseButtonMenu<Button, Inventory.Slot>, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    protected Inventory MobInventory;

    public override void _Ready()
    {
        base._Ready();
		Container ??= (Control)FindChild("EQUIPMENT");

    }

    public void Update(Mob mob)
    {
        MobInventory = mob.MobInventory;
        Update(MobInventory.GetSlots());
    }

    protected override void OnButtonPressed(Button button, Inventory.Slot slot)
    {
    }

    protected override void OnButtonHovered(Button button, Inventory.Slot slot, bool hovered)
    {
        button.Modulate = hovered ? new Godot.Color(0.5f, 0.5f, 0.5f) : new Godot.Color(1, 1, 1);
    }

    protected override void OnButtonCreated(Button button, Inventory.Slot slot)
    {
        if(slot.Item is not null) {button.Text = slot.Item.Name;}
        else if (slot.FlagWhitelist.Count != 0) {button.Text = slot.FlagWhitelist[0].ToString();}
        else {button.Text = "EMPTY";}
    }

}
