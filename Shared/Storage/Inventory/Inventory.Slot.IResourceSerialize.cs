using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Inventory
{
    public partial class Slot :  IResourceSerialize<Slot, InventorySlotResource>
    {
        public InventorySlotResource ToResource()
        {
            InventorySlotResource output = new();

            output.FlagBlacklist.AddRange(FlagBlacklist);
            output.FlagWhitelist.AddRange(FlagWhitelist);
            output.Item = Item?.ToResource();

            return output;
        }

        public static Slot FromResource(InventorySlotResource resource)
        {
            Slot output = new();

            output.FlagBlacklist.AddRange(from flag in resource.FlagBlacklist select flag);
            output.FlagWhitelist.AddRange(from flag in resource.FlagWhitelist select flag);
            output.Item = resource.Item is not null ? Item.FromResource(resource.Item) : null;

            return output;
        }

    } 

}

