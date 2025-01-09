using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared.Storage;

public class Trinket : Item
{
    public Trinket()
    {
        Name = "Trinket";
        Value = 0;
        Flags = new(){EItemFlag.ACCESSORY};
    }
}
