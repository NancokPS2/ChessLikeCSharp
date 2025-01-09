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
            EJob.WARRIOR => output.ChainWarrior(),
            EJob.WIZARD => output.ChainWizard(),
            EJob.RANGER => output.ChainDefaultStats(),
            _ => output.ChainDefaultStats(),
        };
        return output.ChainIdentifier(identity_enum);
    }

public Job ChainDefaultStats()
    {
        StatMultiplicativeBoostDict.Add(StatName.HEALTH, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.ENERGY, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.MOVEMENT, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.JUMP, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.DELAY, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.STRENGTH, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.AGILITY, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.INTELLIGENCE, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.DEFENSE, 1.0f);
        Abilities.Add(new AbilityWeaponAttack());
        return this;
    }

    public Job ChainWarrior()
    {
        StatMultiplicativeBoostDict.Add(StatName.HEALTH, 1.2f);
        StatMultiplicativeBoostDict.Add(StatName.ENERGY, 0.8f);
        StatMultiplicativeBoostDict.Add(StatName.STRENGTH, 1.3f);
        StatMultiplicativeBoostDict.Add(StatName.AGILITY, 1.0f);
        StatMultiplicativeBoostDict.Add(StatName.INTELLIGENCE, 0.8f);
        StatMultiplicativeBoostDict.Add(StatName.DELAY, 1.1f);
        return this;
    }

    public Job ChainWizard()
    {
        StatMultiplicativeBoostDict.Add(StatName.HEALTH, 0.7f);
        StatMultiplicativeBoostDict.Add(StatName.ENERGY, 1.5f);
        StatMultiplicativeBoostDict.Add(StatName.STRENGTH, 0.6f);
        StatMultiplicativeBoostDict.Add(StatName.AGILITY, 0.9f);
        StatMultiplicativeBoostDict.Add(StatName.INTELLIGENCE, 1.4f);
        StatMultiplicativeBoostDict.Add(StatName.DELAY, 1.2f);
        return this;
    }

    public Job ChainIdentifier(EJob identifier)
    {
        Identifier = identifier;
        return this;
    }

}
