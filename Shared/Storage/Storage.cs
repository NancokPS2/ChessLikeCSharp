namespace ChessLike.Shared;

public partial class Inventory
{
    public enum Error
    {
        NONE,
        NO_SPACE,       //Add: Not enough slots left to fit the item.
        SLOT_OCCUPIED,  //Add: Another item is in the slot.
        NO_VALID_SLOT,      //Remove: No slot in this inventory can hold this item.
        INVALID_SLOT,   //A forced attempt was made to add the item to an invalid slot.
        ITEM_NOT_INSIDE, //Tried to interact with an item that's not in this inventory.
        FATAL,          //If something that's not supposed to happen, happens.

    }
     /*
    [Flags]
    public enum ModFlag
    {
        REPLACE = 1 << 0,//Delete the item from the targeted slot if any is present.
        INVENTORY_FALLBACK = 1 << 1,//Add the item to the linked inventory if it is not possible to add it.
    } */

    public const int INVALID_SLOT = -1;

    private List<Slot> Slots = new();

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
        return Error.NO_VALID_SLOT;
    }

    public Error AddItem(Item item_to_add, Slot slot)
    {

        //The lost must be able to hold it
        if (!slot.IsItemValid(item_to_add))
        {
            return Error.INVALID_SLOT;
        }

        //Fail if there's not enough slots.
        if(GetFreeSlots() <= 0)
        {
            return Error.NO_SPACE;
        }

        //Finally add the item.
        slot.Item = item_to_add;
        return Error.NONE;
    }

    public Error RemoveItem(Item item)
    {
        
        Slot? with_item = GetSlotWithItem(item);
        if (with_item is Slot not_null)
        {
            not_null.Item = null;
            return Error.NONE;
        }
        else
        {
            return Error.ITEM_NOT_INSIDE;
        }
    }

    public Error RemoveItem(Slot slot)
    {
        slot.Item = null;
        return Error.NONE;
    }

    public bool ContainsItem(Item item) => Slots.Any( x => x.Item == item);

    public List<Slot> GetSlotsEmpty() => Slots.Where(x => x.Item is null).ToList();

    public Slot? GetSlotWithItem(Item item)
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


}
