using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob
{
    public class MobInventory : Inventory, StatSet<StatName>.IStatBooster
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

        public string GetBoostSource() => "EQUIPMENT";

        public StatSet<StatName>.StatBoost GetStatBoost()
        {
            StatSet<StatName>.StatBoost output = new(GetBoostSource());
            foreach (var slot in GetSlots())
            {
                if (slot.Item is Equipment equip)
                {
                    var boost = equip.GetStatBoost();
                    if (boost is not null)
                    {
                        output += boost;
                    } 
                        
                }
            }

            return output;
        }

    }
}
