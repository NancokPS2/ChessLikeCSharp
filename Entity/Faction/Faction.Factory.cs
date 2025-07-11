using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;

namespace ChessLike.Entity;

public partial class Faction
{
    [Obsolete("EFaction.PLAYER does not work.")]
    public static Faction CreatePrototype(EFaction faction_enum)
    {
        Faction output = new();
        output = faction_enum switch
        {
            EFaction.PLAYER => new (){Identifier = EFaction.PLAYER},
            EFaction.NEUTRAL => new (){Identifier = EFaction.NEUTRAL},
            _ => output,
        };
        return output.ChainIdentifier(faction_enum);
    }  

    public Faction ChainIdentifier(EFaction faction)
    {
        Identifier = faction;
        return this;
    }

}
