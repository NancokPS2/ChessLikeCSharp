using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace ChessLike.Shared.Storage;

public partial class Inventory
{
    public enum Error
    {
        NONE,
        FATAL,          //If something that's not supposed to happen, happens.

        REMOVE_ITEM_NOT_IN_INVENTORY, //Tried to take an item, but nothing was inside the inventory.
        REMOVE_SLOT_ALREADY_EMPTY,

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
            return Error.REMOVE_ITEM_NOT_IN_INVENTORY;
        }
    }

    public Error RemoveItem(Slot slot)
    {
        if (slot.Item is null){return Error.REMOVE_SLOT_ALREADY_EMPTY;}
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

    private enum TransferMode {EXCHANGE, SEND_TO_TARGET, TAKE_FROM_TARGET }
    public Error TransferItem(Slot source_slot, Inventory target_inv, Slot target_slot) => TransferItem(this, source_slot, target_inv, target_slot);
    public static Error TransferItem(Inventory source_inv, Slot source_slot, Inventory target_inv, Slot target_slot)
    {
        if (!source_inv.ContainsSlot(source_slot)){throw new ArgumentException("The source slot must be inside the source inventory");}
        if (!target_inv.ContainsSlot(target_slot)){throw new ArgumentException("The target slot must be inside the target inventory");}

        Item? source_item = source_slot.Item;
        Item? target_item = target_slot.Item;


        TransferMode mode;
        if (source_item is not null && target_item is null
        && target_slot.IsItemValid(source_item))
            {mode = TransferMode.SEND_TO_TARGET;}
        else if (source_item is null && target_item is not null
        && source_slot.IsItemValid(target_item)
        )
            {mode = TransferMode.TAKE_FROM_TARGET;}
        else if (source_item is not null && target_item is not null 
        && source_slot.IsItemValid(target_item) && target_slot.IsItemValid(source_item))
            {mode = TransferMode.EXCHANGE;}
        else 
            {return Error.ADD_INVALID_SLOT;}

        switch (mode)
        {
            case TransferMode.EXCHANGE:
                //Remove the items.
                ThrowOnError(source_inv.RemoveItem(source_slot));
                ThrowOnError(target_inv.RemoveItem(target_slot));

                //Add the items.
                ThrowOnError(target_inv.AddItem(source_item, target_slot));
                ThrowOnError(source_inv.AddItem(target_item, source_slot));
                break;

            case TransferMode.SEND_TO_TARGET:
                ThrowOnError(source_inv.RemoveItem(source_slot));

                ThrowOnError(target_inv.AddItem(source_item, target_slot));
                break;
            
            case TransferMode.TAKE_FROM_TARGET:
                ThrowOnError(target_inv.RemoveItem(target_slot));

                ThrowOnError(source_inv.AddItem(target_item, source_slot));
                break;

            default:
                throw new NotImplementedException();
        }



        return Error.NONE;
    }

    private static void ThrowOnError(Error error, List<Error>? to_ignore = null)
    {
        if(error != Error.NONE || (!to_ignore?.Contains(error) ?? false))
        {
            throw new Exception("Failed due to error " + error.ToString());
        }
    }


}
