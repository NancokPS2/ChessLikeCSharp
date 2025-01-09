using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action.Preset;

namespace ChessLike.Shared.Storage;

public class WeaponSpear: Item
{
    public WeaponSpear()
    {
        Name = "Spear";
        Abilities = new(){new AbilityWeaponAttack(AbilityWeaponAttack.AbilityVariant.SPEAR, 15)};
        Value = 100;
        Flags = new(){EItemFlag.WEAPON, EItemFlag.TWO_HANDED};
    }
}
