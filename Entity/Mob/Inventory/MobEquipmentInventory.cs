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
	public delegate void InventoryChange(MobEquipmentInventory inventory);
	public event InventoryChange? InventoryChanged;
    protected List<EMobEquipmentSlot> BlockedSlots = new();

    protected Dictionary<EMobEquipmentSlot, Item?> Contents;
    [Export]
    private Godot.Collections.Dictionary<EMobEquipmentSlot, Item?> contents
    {
        set => Contents = new(value);
        get => new(Contents);
    }

    public MobEquipmentInventory()
    {
        Contents = new()
        {
            {EMobEquipmentSlot.LEFT_HAND, null},
            {EMobEquipmentSlot.RIGHT_HAND, null},
            {EMobEquipmentSlot.HELMET, null},
            {EMobEquipmentSlot.ARMOR, null},
            {EMobEquipmentSlot.ACCESSORY_1, null},
            {EMobEquipmentSlot.ACCESSORY_2, null},
        };
    }

    public bool CanUseSlot(EMobEquipmentSlot slot)
        => !BlockedSlots.Contains(slot);

    public bool IsValidForSlot(Item item, EMobEquipmentSlot slot)
        => slot switch
        {
            EMobEquipmentSlot.LEFT_HAND => item.Flags.Contains(EItemFlag.WEAPON),
            EMobEquipmentSlot.RIGHT_HAND => item.Flags.Contains(EItemFlag.WEAPON),
            EMobEquipmentSlot.HELMET => item.Flags.Contains(EItemFlag.HELMET),
            EMobEquipmentSlot.ARMOR => item.Flags.Contains(EItemFlag.ARMOR),
            EMobEquipmentSlot.ACCESSORY_1 => item.Flags.Contains(EItemFlag.ACCESSORY),
            EMobEquipmentSlot.ACCESSORY_2 => item.Flags.Contains(EItemFlag.ACCESSORY),
            _ => false,
        };

	public void EquipItem(Item item, EMobEquipmentSlot slot, bool replace)
	{
		if (!CanUseSlot(slot))
			throw new Exception();

		if (Contents[slot] is not null && !replace)
			throw new Exception();

		if (!IsValidForSlot(item, slot))
		{
			MsgLog.AddMessage($"{item.Name} does not fit in slot {slot}");
			return;
		}

		Contents[slot] = item;
		InventoryChanged?.Invoke(this);
    }

	public void UnequipItem(EMobEquipmentSlot slot)
	{
		Contents[slot] = null;
		InventoryChanged?.Invoke(this);
	}

	public Item? GetItem(EMobEquipmentSlot slot)
        => Contents[slot];

    public List<Item> GetItems()
        => [.. Contents.Values.Where(x => x is not null)];

    public EMobEquipmentSlot[] GetSlots()
        => Enum.GetValues<EMobEquipmentSlot>();

}
