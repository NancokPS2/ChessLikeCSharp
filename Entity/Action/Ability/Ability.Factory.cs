using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class Ability
{
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