using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Shared.Storage;

public class Equipment : Item, StatSet<StatName>.IStatBooster
{
    
    public Equipment() : base()
    {
        Flags = new(){EItemFlag.EQUIPMENT};
    }

}
