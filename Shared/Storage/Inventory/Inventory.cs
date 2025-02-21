using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class Inventory
{
    public enum Error
    {
        NONE,
        UNHANDLED,          //If something that's not supposed to happen, happens.

        REMOVE_ITEM_NOT_IN_INVENTORY, //Tried to take an item, but nothing was inside the inventory.
        REMOVE_SLOT_ALREADY_EMPTY,

        ADD_NO_SPACE,       //Add: Not enough slots left to fit the item.
        ADD_INVALID_SLOT,   //A forced attempt was made to add the item to an invalid slot.
        TRANSFER_FAILED,    //Any problem related to a transfer.
    }

    public const int INVALID_SLOT = -1;

    /// <summary>
    /// When this is used as a slot and StorageInventory is true, the item is added regardless of existing slots. Maybe don't give this slot an item.
    /// </summary>
    public Slot StorageInventoryUniversalSlot = new();
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

    #region Slot
    public void AddSlot(Slot slot)
    {
        Slots.Add(slot);
        EventBus.InventoryChanged?.Invoke(this);
    }

    public void RemoveSlot(Slot slot)
    {
        Slots.Remove(slot);
        EventBus.InventoryChanged?.Invoke(this);
    }

    public List<Slot> GetSlots() => Slots;

    public Slot? GetSlotForItem(Item item, bool must_be_empty)
    {
        List<Slot> list = must_be_empty ? GetSlotsEmpty() : GetSlots();
        return list.First(x => x.IsItemValid(item));
    }

    public bool ContainsSlot(Slot slot) => Slots.Any(x => x == slot);

    public List<Slot> GetSlotsEmpty() => Slots.Where(x => x.Item is null).ToList();

    public void ClearEmptySlots()
    {
        Slots.RemoveAll(x => x.Item is null);
    }

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
    #endregion
    
    #region Item
    public Item? GetItem(int slot)
    {
        return Slots[slot].Item;
    }

    public List<Item> GetItems() 
        => (
            from slot 
            in GetSlots().Where(x => x.Item != null) 
            select slot.Item
            ).ToList();

    public Error AddItem(Item item_to_add, Slot slot)
    {
        Error err;
        if (!ContainsSlot(slot))
        {
            throw new Exception("This function is meant to be used in one of this inventory's slots.");
        }
        
        //The lost must be able to hold it
        if (!slot.IsItemValid(item_to_add))
        {
            err = Error.ADD_INVALID_SLOT;
            EventBus.InventoryErrored?.Invoke(this, err);
            return err;
        }

        //Fail if there's not enough slots.
        if(GetFreeSlots() <= 0)
        {
            err = Error.ADD_NO_SPACE;
            EventBus.InventoryErrored?.Invoke(this, err);
            return err;
        }

        //Finally add the item.
        slot.Item = item_to_add;
        EventBus.InventoryChanged?.Invoke(this);
        EventBus.InventoryItemAdded?.Invoke(this, slot, item_to_add);
        return Error.NONE;
    }

    public Error AddItem(Item item_to_add)
    {
        Error err;
        //Check if any slot can house the item.
        foreach (var slot in Slots)
        {
            if (slot.IsItemValid(item_to_add))
            {
                return AddItem(item_to_add, slot);
            }
        }
        err = Error.ADD_INVALID_SLOT;
        EventBus.InventoryErrored?.Invoke(this, err);
        return err;
    }
    public Error AddItem(Slot source_slot, Slot target_slot)
    {
        if (source_slot.Item is null) throw new ArgumentException("The slot must contain something.");
        return AddItem(source_slot.Item, target_slot);
    }

    public Error RemoveItem(Item item)
    {
        Slot? slot_with_item = FindSlotWithItem(item);
        Error err;
        if (slot_with_item is not null)
        {
            err = RemoveItem(slot_with_item);
        }
        else
        {
            err = Error.REMOVE_ITEM_NOT_IN_INVENTORY;
            EventBus.InventoryErrored?.Invoke(this, err);
        }
        return err;
    }

    public Error RemoveItem(Slot slot)
    {
        if (slot.Item is null){return Error.REMOVE_SLOT_ALREADY_EMPTY;}
        else
        {
            Item item = slot.Item;
            slot.Item = null; 
            EventBus.InventoryChanged?.Invoke(this);
            EventBus.InventoryItemRemoved?.Invoke(this, slot, item);
            return Error.NONE;
        }
    }

    public bool ContainsItem(Item item) => Slots.Any( x => x.Item == item);
    #endregion


    #region Transfer
    private enum TransferMode {EXCHANGE, SEND_TO_TARGET, TAKE_FROM_TARGET }
    public Error TransferItem(Slot source_slot, Inventory target_inv, Slot target_slot) => TransferItem(this, source_slot, target_inv, target_slot);
    public static Error TransferItem(Inventory source_inv, Slot source_slot, Inventory target_inv, Slot target_slot)
    {
        if (!source_inv.ContainsSlot(source_slot)){throw new ArgumentException("The source slot must be inside the source inventory");}
        if (!target_inv.ContainsSlot(target_slot)){throw new ArgumentException("The target slot must be inside the target inventory");}

        //Fetch items for transfer.
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

                if (source_item is null || target_item is null)
                {
                    Error err = Error.TRANSFER_FAILED;
                    EventBus.InventoryErrored?.Invoke(source_inv, err);
                    EventBus.InventoryErrored?.Invoke(target_inv, err);
                    return err;
                }

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
    #endregion

    private static void ThrowOnError(Error error, List<Error>? to_ignore = null)
    {
        if(error != Error.NONE || (!to_ignore?.Contains(error) ?? false))
        {
            throw new Exception("Failed due to error " + error.ToString());
        }
    }


}
