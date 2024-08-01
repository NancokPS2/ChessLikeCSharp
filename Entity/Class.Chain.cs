using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    public Job ChainCombatant()
    {
        stats.SetStat(StatSet.Name.HEALTH, 100);
        stats.SetStat(StatSet.Name.ENERGY, 40);
        stats.SetStat(StatSet.Name.MOVEMENT, 20);
        stats.SetStat(StatSet.Name.DELAY, 100);
        actions.Add(new Action().ChainDealDamage(10));
        return this;
    }
}
