using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;
using System;

[GlobalClass]
public partial class MobEquipmentUI : Control, ISceneDependency
{
    public string SCENE_PATH { get; } = "res://Godot/Display/UI/Mob/MobEquipmentUI.tscn";

    [Export]
	public Control? NodeEquipContainer;

    public override void _Ready()
    {
        base._Ready();
		NodeEquipContainer ??= (Control)FindChild("EQUIPMENT");

    }

    public void Update(Mob mob)
    {
        NodeEquipContainer.FreeChildren();

        foreach (var slot in mob.Inventory.GetSlots())
        {
            Button button = new()
            {
                AnchorRight = 1.0f,
                SizeFlagsHorizontal = SizeFlags.ExpandFill,
                FocusMode = Control.FocusModeEnum.All,
            };
            if(slot.Item is not null) {button.Text = slot.Item.Name;}
            else if (slot.FlagWhitelist.Count != 0) {button.Text = slot.FlagWhitelist[0].ToString();}
            else {button.Text = "EMPTY";}

            button.Pressed += () => OnButtonPressed(button, slot);
            button.MouseEntered += () => OnButtonHovered(button, slot, true);
            button.MouseExited += () => OnButtonHovered(button, slot, false);
            NodeEquipContainer.AddChild(button);
        }
    
    }

    public void OnButtonPressed(Button button, Inventory.Slot slot)
    {

    }

    public void OnButtonHovered(Button button, Inventory.Slot slot, bool hovered)
    {
        button.Modulate = hovered ? new Godot.Color(0.5f, 0.5f, 0.5f) : new Godot.Color(1, 1, 1);
    }
}
