using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace ChessLike.Shared.Storage.Items;

public class Armor : Equipment
{
    public Armor()
    {
        StatBoost = new MobStatSet.StatBoost(GetBoostSource());
        StatBoost.SetAdditiveMax(StatName.HEALTH, 10);
    }
}
