using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;

public partial class Faction : ISerializable
{
    public string GetFileName()
    {
        return Enum.GetName(Identifier) + ".xml";
    }

    public string GetSubDirectory()
    {
        return "factions";
    }
}
