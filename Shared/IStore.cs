using ChessLike.Storage;

namespace ChessLike.Shared;

public interface IStore
{
    public Inventory inventory {get; set;}
    public Inventory.Error AddItem(Item item, int slot = Inventory.INVALID_SLOT);
    public Inventory.Error RemoveItem(int slot);
    public Item? GetItem(int slot);
}
