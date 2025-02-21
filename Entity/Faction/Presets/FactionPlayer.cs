using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public class FactionPlayer : Faction
{
    public FactionPlayer()
    {
        Identifier = EFaction.PLAYER;
        Inventory = new Shared.Storage.Inventory(){
            IsInfiniteStorage = true,
            StorageInventoryMaxSlots = 99,
            StorageInventoryUniversalSlot = new()
        };
    }
}
