using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Inventory : IResourceSerialize<Inventory, InventoryResource>
{

    public InventoryResource ToResource()
    {
        throw new NotImplementedException();
    }

    public static Inventory FromResource(InventoryResource resource)
    {
        Inventory output = new();

        output.Slots.AddRange(from slot in resource.Slots select Slot.FromResource(slot));

        return output;
    }
}

