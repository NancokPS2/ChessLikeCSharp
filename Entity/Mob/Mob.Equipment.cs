using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;
using Godot;

namespace ChessLike.Entity;

public partial class Mob
{
    public void EquipmentAdd(Item equip)
    {
        var err = MobInventory.AddItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to equip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }
        
        Stats.BoostAdd(MobInventory, true);
    }

    public void EquipmentRemove(Item equip)
    {
        var err = MobInventory.RemoveItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to unequip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }

        Stats.BoostAdd(MobInventory, true);
    }   
}
