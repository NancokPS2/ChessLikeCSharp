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
            EFaction.PLAYER => output.ChainBasicInventory(),
            EFaction.NEUTRAL => output.ChainBasicInventory(),
            _ => output,
        }; 
        return output.ChainIdentifier(faction_enum);
    }  

    public Faction ChainBasicInventory()
    {
        Inventory.AddItem(new Trinket(){Name = "Trinket"});
        return this;
    }
    public Faction ChainIdentifier(EFaction faction)
    {
        Identifier = faction;
        return this;
    }

}
