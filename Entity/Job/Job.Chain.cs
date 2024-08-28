using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    protected Job ChainDefaultStats()
    {
        Stats.SetStat(StatName.HEALTH, 100);
        Stats.SetStat(StatName.ENERGY, 40);
        Stats.SetStat(StatName.MOVEMENT, 5);
        Stats.SetStat(StatName.DELAY, 100);
        Stats.SetStat(StatName.STRENGTH, 100);
        Stats.SetStat(StatName.AGILITY, 100);
        Stats.SetStat(StatName.INTELLIGENCE, 100);
        return this;
    }

    protected Job ChainWarrior()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatName.HEALTH, 1.2);
        Stats.MultiplyStat(StatName.ENERGY, 0.8);
        Stats.MultiplyStat(StatName.STRENGTH, 1.3);
        Stats.MultiplyStat(StatName.AGILITY, 1.0);
        Stats.MultiplyStat(StatName.INTELLIGENCE, 0.8);
        actions.Add(Action.Loader.Preset.BasicAttack());
        return this;
    }

    protected Job ChainWizard()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatName.HEALTH, 0.7);
        Stats.MultiplyStat(StatName.ENERGY, 1.5);
        Stats.MultiplyStat(StatName.STRENGTH, 0.6);
        Stats.MultiplyStat(StatName.AGILITY, 0.9);
        Stats.MultiplyStat(StatName.INTELLIGENCE, 1.4);
        actions.Add(Action.Loader.Preset.BasicAttack());
        return this;
    }
  
}
