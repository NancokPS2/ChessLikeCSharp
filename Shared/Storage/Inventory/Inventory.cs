namespace ChessLike.Shared.Storage;

public partial class Inventory
{
    public enum Error
    {
        NONE,
        FATAL,          //If something that's not supposed to happen, happens.

        REMOVE_ITEM_NOT_INSIDE, //Tried to interact with an item that's not in this inventory.
        REMOVE_ALREADY_EMPTY,

        ADD_NO_SPACE,       //Add: Not enough slots left to fit the item.
        ADD_INVALID_SLOT,   //A forced attempt was made to add the item to an invalid slot.
    }
     /*
    [Flags]
    public enum ModFlag
    {
        REPLACE = 1 << 0,//Delete the item from the targeted slot if any is present.
        INVENTORY_FALLBACK = 1 << 1,//Add the item to the linked inventory if it is not possible to add it.
    } */

    public const int INVALID_SLOT = -1;

    protected List<Slot> Slots = new();

    public Inventory()
    {
    }

    public Inventory(int size)
    {
        for (int i = size; i < size; i++)
        {
            Slots.Add(new());
        }
    }


    public void AddSlot(Slot slot)
    {
        Slots.Add(slot);
    }

    public List<Slot> GetSlots() => Slots;

    public Item? GetItem(int slot)
    {

        return Slots[slot].Item;
    }

    public Error AddItem(Item item_to_add, Slot slot)
    {

        //The lost must be able to hold it
        if (!slot.IsItemValid(item_to_add))
        {
            return Error.ADD_INVALID_SLOT;
        }

        //Fail if there's not enough slots.
        if(GetFreeSlots() <= 0)
        {
            return Error.ADD_NO_SPACE;
        }

        //Finally add the item.
        slot.Item = item_to_add;
        return Error.NONE;
    }

    public Error AddItem(Item item_to_add)
    {
        //Check if any slot can house the item.
        foreach (var slot in Slots)
        {
            if (slot.IsItemValid(item_to_add))
            {
                return AddItem(item_to_add, slot);
            }
        }
        return Error.ADD_INVALID_SLOT;
    }
    public Error AddItem(Slot slot)
    {
        if (slot.Item is null){throw new ArgumentException("The slot must contain something.");}
        return AddItem(slot.Item);
    }

    public Error AddItem(Slot source_slot, Slot target_slot)
    {
        if (source_slot.Item is null){throw new ArgumentException("The slot must contain something.");}
        return AddItem(source_slot.Item, target_slot);
    }

    public Error RemoveItem(Item item)
    {
        Slot? with_item = FindSlotWithItem(item);
        if (with_item is not null)
        {
            return RemoveItem(with_item);
        }
        else
        {
            return Error.REMOVE_ITEM_NOT_INSIDE;
        }
    }

    public Error RemoveItem(Slot slot)
    {
        if (slot.Item is not null){return Error.REMOVE_ALREADY_EMPTY;}
        else
        {
            slot.Item = null; 
            return Error.NONE;
        }
    }

    public bool ContainsItem(Item item) => Slots.Any( x => x.Item == item);

    public bool ContainsSlot(Slot slot) => Slots.Any(x => x == slot);

    public List<Slot> GetSlotsEmpty() => Slots.Where(x => x.Item is null).ToList();

    /// <summary>
    /// Returns a slot containing the provided item.
    /// </summary>
    /// <param name="item">The item to look for.</param>
    /// <returns>The slot with the item, or null if none are found in this inventory.</returns>
    public Slot? FindSlotWithItem(Item item)
    {
        if (!ContainsItem(item))
        {
            return null;
        }
        
        return Slots.First( x => x.Item == item);
    }
    
    public int GetFreeSlots() => Slots.Count( x => x.Item is null);



    public bool IsSlotEmpty(int slot)
    {
        return GetItem(slot) == null;
    }

    public Error TransferItem(Item item, Inventory target_inv) => TransferItem(this, item, target_inv);
    public Error TransferItem(Slot slot, Inventory target_inv) => TransferItem(this, slot, target_inv);

    public static Error TransferItem(Inventory source_inv, Item item, Inventory target_inv) => TransferItem(source_inv, source_inv.FindSlotWithItem(item), target_inv);

    public static Error TransferItem(Inventory source_inv, Slot slot, Inventory target_inv)
    {
        if (!source_inv.ContainsSlot(slot)){throw new ArgumentException("The slot must be inside the source inventory");}

        Error remove_err = source_inv.RemoveItem(slot);
        if (remove_err != Error.NONE)
        {
            return remove_err;
        } 

        Error add_err = target_inv.AddItem(slot);
        if (add_err != Error.NONE)
        {
            return add_err;
        } 

        return Error.NONE;
    }


}
