using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Ability
{

    public static Ability Create(EAbility identity_enum)
    {
        Ability output = new();
        output = identity_enum switch
        {
            EAbility.PUNCH => output
                .ChainName("Punch")
                .ChainEffectDamageHealth(EStatName.STRENGTH, 0.5f),
            EAbility.HEAL => output
                .ChainName("Heal")
                .ChainHealHealth(25),
            EAbility.MOVE => output
                .ChainName("Move")
                .ChainEffectMove(),
            _ => throw new Exception("Non existent enum value."),
        };

        output.ChainIdentifier(identity_enum);
        return output;
        
    }
    public Ability ChainIdentifier(EAbility identifier)
    {
        Identifier = identifier;
        return this;
    }

    public Ability ChainFlag(EActionFlag flag)
    {
        Flags.Add(flag);
        return this;
    }

    public Ability ChainName(string name)
    {
        Name = name;
        return this;
    }

  

    public Ability ChainEffectDamageHealth(EStatName stat_based, float damage)
    {
        //Effect
        EffectStatChange effect = new();
        effect.SetOwnerAddingBoost(EStatName.STRENGTH, 1);

        //Targeting
        TargetParams.Range = 1;

        return this;
    }

    public Ability ChainHealHealth(float amount)
    {
        //Effect
        EffectStatChange effect = new();
        effect.SetFlatAmount(amount);

        //Targeting
        TargetParams.Range = 1;

        return this;
    }

    public Ability ChainEffectMove()
    {
        //TargetParams
        TargetParams.RangeStatBonus = EStatName.MOVEMENT;
        TargetParams.Range = 0;
        TargetParams.UsesPathing = true;
        return this;
    }

    public Ability ChainTargetBoostRangeByStat( EStatName stat)
    {
        TargetParams.RangeStatBonus = stat;
        return this;
    }


}