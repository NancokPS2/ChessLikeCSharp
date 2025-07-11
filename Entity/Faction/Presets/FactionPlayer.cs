using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;

namespace ChessLike.Entity;

public partial class FactionPlayer : Faction
{
    public FactionPlayer()
    {
        Identifier = EFaction.PLAYER;
        Inventory = new Inventory();
        for (int i = 0; i < 99; i++)
        {
            Inventory.AddSlot(new());
        }
        ;
    }
}
