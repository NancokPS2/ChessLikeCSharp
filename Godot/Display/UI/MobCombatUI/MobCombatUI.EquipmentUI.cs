using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot.Display;

public partial class EquipmentUI
{

    public ItemList<Inventory.Slot> EquipList;

    public EquipmentUI(VBoxContainer control_reference)
    {
        EquipList = new(control_reference);
    }

    private void DisplayDummy()
    {
        Mob mob = Mob.CreatePrototype(EMobPrototype.HUMAN);
        mob.Inventory.AddItem(new Coin());
        UpdateEquipment(mob);
    }

    public void UpdateEquipment(Mob mob)
    {
        EquipList.ClearItems();
        GD.Print(mob.Inventory.GetSlots().ToArray());
        foreach (var slot in mob.Inventory.GetSlots())
        {
            var menu_item = new ItemList<Inventory.Slot>.MenuItem().ChainSetContained(slot);

            if(slot.Item is not null) {menu_item.ChainSetText(slot.Item.Name);}
            else if (slot.FlagWhitelist.Count != 0) {menu_item.ChainSetText(slot.FlagWhitelist[0].ToString());}

            EquipList.AddItem( menu_item );
            GD.Print("Nother item.");
        }
    }

}
