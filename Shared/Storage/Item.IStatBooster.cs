using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Shared.Storage;

public partial class Item : MobStatSet.IStatBooster
{
    public string GetBoostSource() => MobStatSet.INVALID_BOOST_SOURCE;

    public MobStatSet.StatBoost? GetStatBoost() => StatBoost;

    public MobStatSet.StatBoost? StatBoost;
}
