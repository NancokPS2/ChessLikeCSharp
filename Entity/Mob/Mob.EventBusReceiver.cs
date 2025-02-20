using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob
{
    public void EventBusSetup()
    {
        EventBus.MobActionAdded += OnMobActionAdded;   
        EventBus.MobActionRemoved += OnMobActionRemoved;

        EventBus.MobEquipmentAdded += OnMobEquipmentAdded;
        EventBus.MobEquipmentRemoved += OnMobEquipmentRemoved;
    }


    private void OnMobActionRemoved(Mob mob, ActionEvent action)
    {
        if (mob == this && action is Ability abil)
        {
            RemoveAbility(abil.Identifier);
        }

    }

    private void OnMobActionAdded(Mob mob, ActionEvent action)
    {
        if (mob == this && action is Ability abil)
        {
            AddAbility(abil);
        }
    }

    private void OnMobEquipmentAdded(Mob mob, Item item, Inventory.Slot slot)
    {
        if (mob == this)
        {
            EquipmentAdd(item, slot);
        }
    }
    
    private void OnMobEquipmentRemoved(Mob mob, Item item, Inventory.Slot slot)
    {
        if (mob == this)
        {
            EquipmentRemove(item);
            
        }
    }
}
