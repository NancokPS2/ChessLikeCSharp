using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Godot;

public partial class PartyMobEquipmentUI : MobEquipmentUI
{
    public static readonly Godot.Color MODULATE_SELECTED = new(0.5f,0.5f,0.5f);
    public static readonly Godot.Color MODULATE_NORMAL = new(1,1,1);
    private Inventory.Slot? SlotSelected;

    protected override void OnButtonPressed(Button button, Inventory.Slot slot)
    {
        if (SlotSelected is null)
        {
            SlotSelected = slot;
            button.Modulate = MODULATE_SELECTED;
        }
        else if (SlotSelected.Item is not null && slot.Item is null)
        {
            if (slot.IsItemValid(SlotSelected.Item))
            {
                Item to_transfer = SlotSelected.Item;
                slot.Item = to_transfer;
                button.Modulate = MODULATE_NORMAL;
                Update();
            }
        }
    }
    
}
