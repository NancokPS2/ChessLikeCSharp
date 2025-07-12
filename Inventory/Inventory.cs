using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Storage;

public abstract partial class Inventory : Resource
{

    public const int INVALID_SLOT = -1;

    protected List<ItemFilter> Slots = new();
    [Export]
    private Godot.Collections.Array<ItemFilter> slots
    {
        set => Slots = new(value);
        get => new(Slots);
    }

    public Inventory()
    {
    }

    public Inventory(int size, ItemFilter defaultSlot)
    {
        for (int i = size; i < size; i++)
        {
            Slots.Add(new ItemFilter(defaultSlot));
        }
    }

    #region Slot
    [Obsolete("Turn back to protected later")]
    public List<ItemFilter> GetSlots() => Slots;

    protected ItemFilter? GetSlotThatAllowsItem(Item item, bool must_be_empty)
    {
        List<ItemFilter> list = must_be_empty ? GetSlotsEmpty() : GetSlots();
        return list.First(x => x.IsItemValid(item));
    }

    public bool ContainsSlot(ItemFilter slot) => Slots.Contains(slot);

    protected List<ItemFilter> GetSlotsEmpty() => Slots.Where(x => x.IsEmpty()).ToList();

    [Obsolete("Turn back to protected")]
    public void ClearEmptySlots()
        => Slots.RemoveAll(x => x.IsEmpty());

    /// <summary>
    /// Returns a slot containing the provided item.
    /// </summary>
    /// <param name="item">The item to look for.</param>
    /// <returns>The slot with the item, or null if none are found in this inventory.</returns>
    protected ItemFilter? FindSlotWithItem(Item item)
        => Slots.Find(x => x.Item == item);

    protected int GetEmptySlots()
        => Slots.Count(x => x.IsEmpty());
    #endregion

    #region Item
    public List<Item> GetItems()
        => (
            from slot
            in GetSlots().Where(x => !x.IsEmpty())
            select slot.Item
            ).ToList();

    [Obsolete("Turn back to protected later")]
    public EInventoryError AddItem(Item item_to_add, ItemFilter slot)
    {
        EInventoryError err;
        if (!ContainsSlot(slot))
        {
            throw new Exception("This function is meant to be used in one of this inventory's slots.");
        }

        //The lost must be able to hold it
        if (!slot.IsItemValid(item_to_add))
        {
            err = EInventoryError.ADD_INVALID_SLOT;
            return err;
        }

        //Fail if there's not enough slots.
        if (GetEmptySlots() <= 0)
        {
            err = EInventoryError.ADD_NO_SPACE;
            return err;
        }

        //Finally add the item.
        slot.Item = item_to_add;
        return EInventoryError.NONE;
    }

    [Obsolete("Turn back to protected later")]
    public EInventoryError RemoveItem(ItemFilter slot)
    {
        if (slot.Item is null) { return EInventoryError.REMOVE_SLOT_ALREADY_EMPTY; }
        else
        {
            Item item = slot.Item;
            slot.Item = null;
            return EInventoryError.NONE;
        }
    }
    #endregion


    #region Transfer
    private enum TransferMode { EXCHANGE, SEND_TO_TARGET, TAKE_FROM_TARGET }
    public EInventoryError TransferItem(ItemFilter source_slot, Inventory target_inv, ItemFilter target_slot) => TransferItem(this, source_slot, target_inv, target_slot);
    public static EInventoryError TransferItem(Inventory source_inv, ItemFilter source_slot, Inventory target_inv, ItemFilter target_slot)
    {
        if (!source_inv.ContainsSlot(source_slot)) { throw new ArgumentException("The source slot must be inside the source inventory"); }
        if (!target_inv.ContainsSlot(target_slot)) { throw new ArgumentException("The target slot must be inside the target inventory"); }

        //Fetch items for transfer.
        Item? source_item = source_slot.Item;
        Item? target_item = target_slot.Item;


        TransferMode mode;
        if (source_item is not null && target_item is null
        && target_slot.IsItemValid(source_item))
        { mode = TransferMode.SEND_TO_TARGET; }
        else if (source_item is null && target_item is not null
        && source_slot.IsItemValid(target_item)
        )
        { mode = TransferMode.TAKE_FROM_TARGET; }
        else if (source_item is not null && target_item is not null
        && source_slot.IsItemValid(target_item) && target_slot.IsItemValid(source_item))
        { mode = TransferMode.EXCHANGE; }
        else
        { return EInventoryError.ADD_INVALID_SLOT; }

        switch (mode)
        {
            case TransferMode.EXCHANGE:
                //Remove the items.
                ThrowOnError(source_inv.RemoveItem(source_slot));
                ThrowOnError(target_inv.RemoveItem(target_slot));

                if (source_item is null || target_item is null)
                {
                    EInventoryError err = EInventoryError.TRANSFER_FAILED;
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



        return EInventoryError.NONE;
    }
    #endregion

    private static void ThrowOnError(EInventoryError error, List<EInventoryError>? to_ignore = null)
    {
        if (error != EInventoryError.NONE || (!to_ignore?.Contains(error) ?? false))
        {
            throw new Exception("Failed due to error " + error.ToString());
        }
    }


}
