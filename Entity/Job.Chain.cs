using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    public Job ChainDefaultStats()
    {
        stats.SetStat(StatSet.Name.HEALTH, 100);
        stats.SetStat(StatSet.Name.ENERGY, 40);
        stats.SetStat(StatSet.Name.MOVEMENT, 5);
        stats.SetStat(StatSet.Name.DELAY, 100);
        stats.SetStat(StatSet.Name.STRENGTH, 100);
        stats.SetStat(StatSet.Name.AGILITY, 100);
        stats.SetStat(StatSet.Name.INTELLIGENCE, 100);
        return this;
    }

    public Job ChainWarrior()
    {
        this.ChainDefaultStats();

        stats.MultiplyStat(StatSet.Name.HEALTH, 1.2);
        stats.MultiplyStat(StatSet.Name.ENERGY, 0.8);
        stats.MultiplyStat(StatSet.Name.STRENGTH, 1.3);
        stats.MultiplyStat(StatSet.Name.AGILITY, 1.0);
        stats.MultiplyStat(StatSet.Name.INTELLIGENCE, 0.8);
        actions.Add(Action.Preset.BasicAttack());
        return this;
    }

    public Job ChainWizard()
    {
        this.ChainDefaultStats();

        stats.MultiplyStat(StatSet.Name.HEALTH, 0.7);
        stats.MultiplyStat(StatSet.Name.ENERGY, 1.5);
        stats.MultiplyStat(StatSet.Name.STRENGTH, 0.6);
        stats.MultiplyStat(StatSet.Name.AGILITY, 0.9);
        stats.MultiplyStat(StatSet.Name.INTELLIGENCE, 1.4);
        actions.Add(Action.Preset.BasicAttack());
        return this;
    }
}
