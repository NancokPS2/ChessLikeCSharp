using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public partial class Inventory
{
    public class Slot
    {
        public List<EItemFlag> FlagWhitelist = new();
        public List<EItemFlag> FlagBlacklist = new();
        public Item? Item;

        public Slot()
        {
            FlagWhitelist = new();
            FlagBlacklist = new();
            Item = null;
        }

        public Slot(List<EItemFlag> flag_wl, List<EItemFlag> flag_bl, Item? item = null)
        {
            FlagWhitelist = flag_wl;
            FlagBlacklist = flag_bl;
            Item = item;
        }

        public bool IsItemValid(Item item)
        {
            bool blacklist_clear = FlagBlacklist.Count == 0 || !FlagBlacklist.Any(x => item.Flags.Contains(x));
            bool whitelist_clear = FlagWhitelist.Count == 0 || FlagWhitelist.All( x => item.Flags.Contains(x));

            return blacklist_clear && whitelist_clear;
        }
    }

}
