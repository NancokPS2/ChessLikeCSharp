using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Inventory : IResourceSerialize<Inventory, InventoryResource>
{
    public enum EPreset {EQUIPMENT}
    public static InventoryResource LoadPreset(EPreset preset)
    {
        switch (preset)
        {
            case EPreset.EQUIPMENT:
                return GD.Load<InventoryResource>("res://Resources/EquipmentInventory.tres");
            
            default:
                throw new NotImplementedException();
        }
    }

    public InventoryResource ToResource()
    {
        InventoryResource output = new();

        output.Slots.AddRange(from slot in Slots select slot.ToResource());

        return output;
    }

    public static Inventory FromResource(InventoryResource resource)
    {
        Inventory output = new();

        output.Slots.AddRange(from slot in resource.Slots select Slot.FromResource(slot));

        return output;
    }
}

