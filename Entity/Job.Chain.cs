using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    public Job ChainDefaultStats()
    {
        Stats.SetStat(StatSet.Name.HEALTH, 100);
        Stats.SetStat(StatSet.Name.ENERGY, 40);
        Stats.SetStat(StatSet.Name.MOVEMENT, 5);
        Stats.SetStat(StatSet.Name.DELAY, 100);
        Stats.SetStat(StatSet.Name.STRENGTH, 100);
        Stats.SetStat(StatSet.Name.AGILITY, 100);
        Stats.SetStat(StatSet.Name.INTELLIGENCE, 100);
        return this;
    }

    public Job ChainWarrior()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatSet.Name.HEALTH, 1.2);
        Stats.MultiplyStat(StatSet.Name.ENERGY, 0.8);
        Stats.MultiplyStat(StatSet.Name.STRENGTH, 1.3);
        Stats.MultiplyStat(StatSet.Name.AGILITY, 1.0);
        Stats.MultiplyStat(StatSet.Name.INTELLIGENCE, 0.8);
        actions.Add(Action.Preset.BasicAttack());
        return this;
    }

    public Job ChainWizard()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatSet.Name.HEALTH, 0.7);
        Stats.MultiplyStat(StatSet.Name.ENERGY, 1.5);
        Stats.MultiplyStat(StatSet.Name.STRENGTH, 0.6);
        Stats.MultiplyStat(StatSet.Name.AGILITY, 0.9);
        Stats.MultiplyStat(StatSet.Name.INTELLIGENCE, 1.4);
        actions.Add(Action.Preset.BasicAttack());
        return this;
    }
}
