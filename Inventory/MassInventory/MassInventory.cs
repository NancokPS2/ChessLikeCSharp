using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Storage;

[GlobalClass]
public partial class MassInventory : Resource, IInventory
{
    protected List<Item> Contents = new();
    [Export]
    private Godot.Collections.Array<Item> contents
    {
        set => Contents = new(value);
        get => new(Contents);
    }
    
    [Export]
    protected ItemFilter Filter = new();

    public void AddItem(Item item)
    {
        if (!Filter.IsItemValid(item)) return;

        Contents.Add(item);
    }

    public bool RemoveItem(Item item)
    {
        return Contents.Remove(item);
    }

    public List<Item> GetItems()
    {
        return new(Contents);
    }

}
