using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Faction
{
    public static Faction CreatePrototype(EFaction faction_enum)
    {
        Faction output = new();
        output = faction_enum switch
        {
            EFaction.PLAYER => output.ChainBasicInventory()
        }; 
        return output;
    }  

    public Faction ChainBasicInventory()
    {
        Inventory.AddItem(new Trinket(){Name = "Trinket"});
        return this;
    }

}
