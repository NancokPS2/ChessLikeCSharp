using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Shared.Storage;

public class Equipment : Item, MobStatSet.IStatBooster
{
    
    public Equipment() : base()
    {
        Flags = new(){EItemFlag.EQUIPMENT};
    }

}
