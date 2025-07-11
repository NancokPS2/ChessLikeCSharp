namespace ChessLike.Storage;

public abstract partial class Inventory
{
    public enum EInventoryError
    {
        NONE,
        UNHANDLED,          //If something that's not supposed to happen, happens.

        REMOVE_ITEM_NOT_IN_INVENTORY, //Tried to take an item, but nothing was inside the inventory.
        REMOVE_SLOT_ALREADY_EMPTY,

        ADD_NO_SPACE,       //Add: Not enough slots left to fit the item.
        ADD_INVALID_SLOT,   //A forced attempt was made to add the item to an invalid slot.
        TRANSFER_FAILED,    //Any problem related to a transfer.
    }


}
