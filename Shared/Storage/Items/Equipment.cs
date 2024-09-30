using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Shared.Storage;

public class Equipment : Item, StatSet<StatName>.IStatBooster
{
    public StatSet<StatName>.StatBoost? StatBoost;
    
    public Equipment() : base()
    {
        Flags = new(){EItemFlag.EQUIPMENT};
    }

    public string GetBoostSource() => StatSet<StatName>.INVALID_BOOST_SOURCE;

    public StatSet<StatName>.StatBoost? GetStatBoost() => StatBoost;
}
