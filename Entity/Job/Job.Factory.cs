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
        StatMultiplicativeBoostDict.Add(EStatName.HEALTH, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.ENERGY, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.MOVEMENT, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.JUMP, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.DELAY, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.STRENGTH, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.AGILITY, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.INTELLIGENCE, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.DEFENSE, 1.0f);
        Abilities.Add(new AbilityPunch());
        return this;
    }

    public Job ChainWarrior()
    {
        StatMultiplicativeBoostDict.Add(EStatName.HEALTH, 1.2f);
        StatMultiplicativeBoostDict.Add(EStatName.ENERGY, 0.8f);
        StatMultiplicativeBoostDict.Add(EStatName.STRENGTH, 1.3f);
        StatMultiplicativeBoostDict.Add(EStatName.AGILITY, 1.0f);
        StatMultiplicativeBoostDict.Add(EStatName.INTELLIGENCE, 0.8f);
        StatMultiplicativeBoostDict.Add(EStatName.DELAY, 1.1f);
        return this;
    }

    public Job ChainWizard()
    {
        StatMultiplicativeBoostDict.Add(EStatName.HEALTH, 0.7f);
        StatMultiplicativeBoostDict.Add(EStatName.ENERGY, 1.5f);
        StatMultiplicativeBoostDict.Add(EStatName.STRENGTH, 0.6f);
        StatMultiplicativeBoostDict.Add(EStatName.AGILITY, 0.9f);
        StatMultiplicativeBoostDict.Add(EStatName.INTELLIGENCE, 1.4f);
        StatMultiplicativeBoostDict.Add(EStatName.DELAY, 1.2f);
        return this;
    }

    public Job ChainIdentifier(EJob identifier)
    {
        Identifier = identifier;
        return this;
    }

}
