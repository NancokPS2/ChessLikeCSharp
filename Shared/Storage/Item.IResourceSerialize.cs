using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Item : IResourceSerialize<Item, ItemResource>
{

    public ItemResource ToResource()
    {
        ItemResource resource = new();
        resource.Name = this.Name;
        resource.Price = this.Price;
        resource.Flags.AddRange(from item in this.Flags select item);
        return resource;
    }

    public static Item FromResource(ItemResource resource)
    {
        Item output = new();
        output.Name = resource.Name;
        output.Price = resource.Price;
        output.Flags.AddRange(
            from item in resource.Flags select item
            );
        return output;

    }

}
