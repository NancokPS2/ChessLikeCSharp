using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Storage;
using Godot;

[GlobalClass]
public partial class PartyInventoryUI : Control
{
    [Export]
    protected MobEquipmentUI? EquipmentUI
    {
        get => equipmentUI;
        set
        {
            if (equipmentUI is not null) equipmentUI.ButtonPressed -= OnEquipmentUIButtonPressed;
            equipmentUI = value;
            if (equipmentUI is not null) equipmentUI.ButtonPressed += OnEquipmentUIButtonPressed;
        }
    }
    private MobEquipmentUI? equipmentUI;

    [Export]
    protected MassInventoryUI MassUI
    {
        get => massUI;
        set
        {
            if (massUI is not null) massUI.ButtonPressed -= OnMassUIButtonPressed;
            massUI = value;
            if (massUI is not null) massUI.ButtonPressed += OnMassUIButtonPressed;
        }
    }
    private MassInventoryUI massUI;

    protected Item? MassInventoryItemSelected;

    public override void _Ready()
    {
        base._Ready();
        EventBus.MobSelected += OnMobSelected;

        MassUI.Update(Global.ManagerFaction.GetPooledByEnum(EFaction.PLAYER).Inventory);
    }

    protected void ClearSelected()
    {
        MassInventoryItemSelected = null;
    }

    #region Event Connection
    private void OnEquipmentUIButtonPressed(Button button, (Item?, MobEquipmentInventory.ESlot) param)
    {
        MobEquipmentInventory mobEquipInv = EquipmentUI.MobEquipmentInventorySelected;
        MassInventory massInv = MassUI.MassInventorySelected;
        Item? equipSelected = mobEquipInv.GetItem(param.Item2);
        Item? massSelected = MassInventoryItemSelected;

        //Transfer from mob equipment to mass inventory. itemSelected is not empty, none is selected in the mass inventory.
        if (equipSelected is not null && massSelected is null)
        {
            if (mobEquipInv.GetItem(param.Item2) != equipSelected)
                throw new Exception("This item is not from this mob's inventory or is not at this slot.");

            mobEquipInv.UnequipItem(param.Item2);
            massInv.AddItem(equipSelected);
            ClearSelected();
        }
        //Transfer from mass inventory to mob equipment. The selected item is an empty slot, but there is a massSelected.
        else if (equipSelected is null && massSelected is not null)
        {
            if (mobEquipInv.GetItem(param.Item2) is not null)
                throw new Exception("This slot IS occupied. What!?");

            if (!mobEquipInv.IsValidForSlot(massSelected, param.Item2))
            {
                MessageQueue.AddMessage("That slot is not valid for this item.");
                return;
            }

            massInv.RemoveItem(massSelected);
            mobEquipInv.EquipItem(massSelected, param.Item2, false);
            ClearSelected();
        }

        MassUI.Update(massInv);
        EquipmentUI.Update(mobEquipInv);
    }

    private void OnMassUIButtonPressed(Button button, Item param)
    {
        MassInventoryItemSelected = param;

    }

    private void OnMobSelected(Mob mob)
    {
        EquipmentUI?.Update(mob.EquipmentInventory);
    }
    #endregion
}
