using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared;
using ChessLike.Shared.Storage;

namespace ChessLike.Shared.Storage;

   
public partial class Inventory : MobStatSet.IStatBooster
{

    public string GetBoostSource() => "EQUIPMENT";

    public MobStatSet.StatBoost GetStatBoost()
    {
        MobStatSet.StatBoost output = new(GetBoostSource());
        foreach (var slot in Slots)
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

