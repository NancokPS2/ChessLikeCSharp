using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Class
{
    public Class PresetCombatant()
    {
        stats.SetStat(StatSet.Name.HEALTH, 100);
        stats.SetStat(StatSet.Name.ENERGY, 40);
        stats.SetStat(StatSet.Name.SPEED, 20);
        actions.Add(new Action().PresetDealDamage(10));
        return this;
    }
}
