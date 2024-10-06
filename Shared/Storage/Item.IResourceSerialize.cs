using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Item : IResourceSerialize<Item, ItemResource>
{
    public Item FromResource(ItemResource resource)
    {
        Item output = new();
        output.Name = resource.Name;
        output.Price = resource.Price;
        output.Flags.AddRange(
            from item in resource.Flags select item
            );
        return output;

    }

    public ItemResource ToResource()
    {
        ItemResource resource = new();
        resource.Name = this.Name;
        resource.Price = this.Price;
        resource.Flags.AddRange(from item in this.Flags select item);
        return resource;
    }


}

public static class IResourceSerializeExtension
{

    public static bool IsValidResource<TObj>(this object @this, Resource resource)
    {
        bool failed = false;
        
        PropertyInfo[] properties = typeof(TObj).GetProperties();

        foreach (var item in properties)
        {
            var res_val_type = resource.Get(item.Name).GetType();
            if (!item.GetType().IsAssignableFrom(res_val_type))
            {
                Console.WriteLine(
                    "Cannot convert from " 
                    + item.GetType().ToString() 
                    + " to " + res_val_type.ToString()
                    + " | " + item.Name);
                failed = true;
            }
        }
        return failed;
    }

}
