using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;

namespace ChessLike.Entity;

public partial class Job
{
    public static Job CreatePrototype(EJob identity_enum)
    {
        Job output = new();
        output = identity_enum switch
        {
            EJob.DEFAULT => output.ChainDefaultStats(),
            EJob.WARRIOR => output.ChainDefaultStats().ChainWarrior(),
            EJob.WIZARD => output.ChainDefaultStats().ChainWizard(),
            EJob.RANGER => output.ChainDefaultStats(),
            _ => output.ChainDefaultStats(),
        };
        return output.ChainIdentifier(identity_enum);
    }

public Job ChainDefaultStats()
    {
        StatMultiplicativeBoostDict.SetStat(StatName.HEALTH, 100);
        StatMultiplicativeBoostDict.SetStat(StatName.ENERGY, 40);
        StatMultiplicativeBoostDict.SetStat(StatName.MOVEMENT, 3);
        StatMultiplicativeBoostDict.SetStat(StatName.JUMP, 2);
        StatMultiplicativeBoostDict.SetStat(StatName.DELAY, 100);
        StatMultiplicativeBoostDict.SetStat(StatName.STRENGTH, 100);
        StatMultiplicativeBoostDict.SetStat(StatName.AGILITY, 100);
        StatMultiplicativeBoostDict.SetStat(StatName.INTELLIGENCE, 100);
        StatMultiplicativeBoostDict.SetStat(StatName.DEFENSE, 0);
        Abilities.Add(new AbilityPunch());
        return this;
    }

    public Job ChainWarrior()
    {
        StatMultiplicativeBoostDict.MultiplyStat(StatName.HEALTH, 1.2);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.ENERGY, 0.8);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.STRENGTH, 1.3);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.AGILITY, 1.0);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.INTELLIGENCE, 0.8);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.DELAY, 1.1);
        return this;
    }

    public Job ChainWizard()
    {
        StatMultiplicativeBoostDict.MultiplyStat(StatName.HEALTH, 0.7);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.ENERGY, 1.5);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.STRENGTH, 0.6);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.AGILITY, 0.9);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.INTELLIGENCE, 1.4);
        StatMultiplicativeBoostDict.MultiplyStat(StatName.DELAY, 1.2);
        return this;
    }

    public Job ChainIdentifier(EJob identifier)
    {
        Identifier = identifier;
        return this;
    }

}
