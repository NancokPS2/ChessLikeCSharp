using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    public static Job Create(EJob identity_enum)
    {
        Job output = new();
        output = identity_enum switch
        {
            EJob.WARRIOR => output.ChainDefaultStats().ChainWarrior(),
            EJob.WIZARD => output.ChainDefaultStats().ChainWizard(),
            EJob.RANGER => output.ChainDefaultStats(),
            _ => output.ChainDefaultStats(),
        };
        return output;
    }

public Job ChainDefaultStats()
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

    public Job ChainWarrior()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatName.HEALTH, 1.2);
        Stats.MultiplyStat(StatName.ENERGY, 0.8);
        Stats.MultiplyStat(StatName.STRENGTH, 1.3);
        Stats.MultiplyStat(StatName.AGILITY, 1.0);
        Stats.MultiplyStat(StatName.INTELLIGENCE, 0.8);
        Stats.MultiplyStat(StatName.DELAY, 1.1);
        actions.Add(Action.Create(EAction.PUNCH));
        return this;
    }

    public Job ChainWizard()
    {
        this.ChainDefaultStats();

        Stats.MultiplyStat(StatName.HEALTH, 0.7);
        Stats.MultiplyStat(StatName.ENERGY, 1.5);
        Stats.MultiplyStat(StatName.STRENGTH, 0.6);
        Stats.MultiplyStat(StatName.AGILITY, 0.9);
        Stats.MultiplyStat(StatName.INTELLIGENCE, 1.4);
        Stats.MultiplyStat(StatName.DELAY, 1.2);
        actions.Add(Action.Create(EAction.PUNCH));
        return this;
    }

    public Job ChainIdentifier(EJob identifier)
    {
        Identifier = identifier;
        return this;
    }

}
