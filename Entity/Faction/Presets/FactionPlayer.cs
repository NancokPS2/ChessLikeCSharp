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
        Inventory = new Shared.Storage.Inventory();
        for (int i = 0; i < 99; i++)
        {
            Inventory.AddSlot(new());
        }
        ;
    }
}
