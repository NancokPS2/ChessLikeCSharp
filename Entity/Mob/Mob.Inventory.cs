using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared;

namespace ChessLike.Entity;

public partial class Mob
{
    public class MobInventory : ChessLike.Shared.Inventory
    {
        public MobInventory()
        {
            AddSlot(
                new Slot(new(){EItemFlag.WEAPON}, new())
            );
            AddSlot(
                new Slot(new(){EItemFlag.WEAPON, EItemFlag.ONE_HANDED}, new())
            );
            AddSlot(
                new Slot(new(){EItemFlag.SUIT}, new())
            );
            AddSlot(
                new Slot(new(){EItemFlag.HELMET}, new())
            );
            AddSlot(
                new Slot(new(){EItemFlag.ACCESSORY}, new())
            );
            AddSlot(
                new Slot(new(){EItemFlag.ACCESSORY}, new())
            );
            AddSlot(
                new Slot()
            );
        }
    }
}
