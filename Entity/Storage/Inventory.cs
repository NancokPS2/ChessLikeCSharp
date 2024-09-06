using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public class Inventory
{
    public enum Error
    {
        NONE,
        TOO_FULL,   //Add: Not enough slots left to fit the item.
        INSUFFICIENT_CAPACITY, //Add: If the inventory is too small to fit the item, even if empty.
        NOT_WHITELISTED,    //Add: The item is specifically not allowed in the target slot due to the whitelist.
        SLOT_OCCUPIED,  //Add: Another item is in the slot.
        NOT_FOUND,      //Remove: The item is not present
        FATAL,          //If something that's not supposed to happen, happens.
    }
     /*
    [Flags]
    public enum ModFlag
    {
        REPLACE = 1 << 0,//Delete the item from the targeted slot if any is present.
        INVENTORY_FALLBACK = 1 << 1,//Add the item to the linked inventory if it is not possible to add it.
    } */

    private Dictionary<int, List<EItem>> slot_whitelist = new();

    public const int INVALID_SLOT = -1;

    public Dictionary<int, Item> contents = new();

    public int capacity = 10;

    public Inventory(int size)
    {
        if (size < 0)
        {
            capacity = int.MaxValue;
        }
        capacity = size;
    }

    public Inventory(Item[] items, int size) : this(size)
    {
        foreach (Item item in items)
        {
            AddItem(item);
        }
    }

    public Item? GetItem(int slot)
    {
        Item? output = null;
        contents.TryGetValue(slot, out output);
        return output;
    }

    public Error AddItem(Item item_to_add, int slot = INVALID_SLOT)
    {
        //Check how many slots are left.
        int slots_free = GetEmptySlotCount();
        Item? item_present = GetItem(slot);
        int slot_used;

        //Fail if the item is not in the whitelist of this slot.
        if(IsItemInSlotWhitelist(item_to_add, slot))
        {
            return Error.NOT_WHITELISTED;
        }

        //Fail if the inventory is too small for the object.
        if (item_to_add.slot_size > capacity)
        {
            return Error.INSUFFICIENT_CAPACITY;
        }

        //Fail if there's not enough slots.
        if(slots_free < item_to_add.slot_size)
        {
            return Error.TOO_FULL;
        }

        //Get a free slot if an invalid one was given.
        if(slot == INVALID_SLOT)
        {
            slot_used = GetEmptySlot();
            //Fail if no slot could be given, this should have been caught in the free slots check.
            if (slot_used == INVALID_SLOT)
            {
                Console.WriteLine("ERROR, could not get an empty slot.");
                return Error.FATAL;
            }
        }

        //Finally add the item.
        contents[slot] = item_to_add;
        return Error.NONE;

    }

    public Error RemoveItem(int slot)
    {
        Error error = Error.NONE;
        bool removed = contents.Remove(slot);
        if(!removed)
        {
            error = Error.NOT_FOUND;
        }
        return error;
    }

    public void SetSlotWhitelist(int slot, List<EItem> type_list)
    {
        slot_whitelist[slot] = type_list;
    }

    public List<EItem> GetSlotWhitelist(int slot)
    {
        return slot_whitelist[slot];
    }

    public int GetEmptySlot()
    {
        foreach(int key in contents.Keys)
        {
            if(IsSlotEmpty(key))
            {
                return key;
            }
        }
        return INVALID_SLOT;
    }

    public List<int> GetEmptySlotList()
    {
        List<int> output = new List<int>();
        foreach(int key in contents.Keys)
        {
            if(IsSlotEmpty(key))
            {
                output.Add(key);
            }
        }
        return output;
    }

    public int GetEmptySlotCount()
    {
        int output = 0;
        foreach (int index in Enumerable.Range(0,capacity).ToArray())
        {
            if(!IsSlotEmpty(index))
            {
                output += 1;
            }
        }
        return output;
    }

    public bool IsItemInSlotWhitelist(Item item, int slot)
    {
        //If no whitelist is defined, all items are allowed.
        if(slot_whitelist[slot].Count == 0)
        {
            return true;
        }

        //Check every type of the item.
        foreach(EItem type in item.GetTypes())
        {
            //If any type is found, allow it.
            if(slot_whitelist[slot].Contains(type))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsSlotEmpty(int slot)
    {
        return GetItem(slot) == null;
    }


}
