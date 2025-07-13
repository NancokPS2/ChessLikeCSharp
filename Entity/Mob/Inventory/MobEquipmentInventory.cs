using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobEquipmentInventory : Resource, IInventory
{
    public enum ESlot { LEFT_HAND, RIGHT_HAND, HELMET, ARMOR, ACCESSORY_1, ACCESSORY_2 }

    protected List<ESlot> BlockedSlots = new();

    protected Dictionary<ESlot, Item?> Contents;
    [Export]
    private Godot.Collections.Dictionary<ESlot, Item?> contents
    {
        set => Contents = new(value);
        get => new(Contents);
    }

    public MobEquipmentInventory()
    {
        Contents = new()
        {
            {ESlot.LEFT_HAND, null},
            {ESlot.RIGHT_HAND, null},
            {ESlot.HELMET, null},
            {ESlot.ARMOR, null},
            {ESlot.ACCESSORY_1, null},
            {ESlot.ACCESSORY_2, null},
        };
    }

    public bool CanUseSlot(ESlot slot)
        => !BlockedSlots.Contains(slot);

    public bool IsValidForSlot(Item item, ESlot slot)
        => slot switch
        {
            ESlot.LEFT_HAND => item.Flags.Contains(EItemFlag.WEAPON),
            ESlot.RIGHT_HAND => item.Flags.Contains(EItemFlag.WEAPON),
            ESlot.HELMET => item.Flags.Contains(EItemFlag.HELMET),
            ESlot.ARMOR => item.Flags.Contains(EItemFlag.ARMOR),
            ESlot.ACCESSORY_1 => item.Flags.Contains(EItemFlag.ACCESSORY),
            ESlot.ACCESSORY_2 => item.Flags.Contains(EItemFlag.ACCESSORY),
            _ => false,
        };

    public void EquipItem(Item item, ESlot slot, bool replace)
    {
        if (!CanUseSlot(slot))
            throw new Exception();

        if (Contents[slot] is not null && !replace)
            throw new Exception();

        if (!IsValidForSlot(item, slot))
        {
            MessageQueue.AddMessage($"{item.Name} does not fit in slot {slot}");
            return;
        }

        Contents[slot] = item;
    }

    public void UnequipItem(ESlot slot)
        => Contents[slot] = null;

    public Item? GetItem(ESlot slot)
        => Contents[slot];

    public List<Item> GetItems()
        => [.. Contents.Values.Where(x => x is not null)];

}
