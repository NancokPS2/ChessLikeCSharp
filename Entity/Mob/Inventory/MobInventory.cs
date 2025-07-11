using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Storage;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobInventory : Inventory
{
    public MobInventory()
    {
        Slots = new()
        {
            new(){FlagWhitelist = new(){EItemFlag.WEAPON}},
            new(){FlagWhitelist = new(){EItemFlag.WEAPON}},
            new(){FlagWhitelist = new(){EItemFlag.HELMET}},
            new(){FlagWhitelist = new(){EItemFlag.ARMOR}},
            new(){FlagWhitelist = new(){EItemFlag.ACCESSORY}},
            new(){FlagWhitelist = new(){EItemFlag.ACCESSORY}},
        };
    }
}
