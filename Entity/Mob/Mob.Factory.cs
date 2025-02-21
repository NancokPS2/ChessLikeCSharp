using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob
{
    /// <summary>
    /// This class should only be used to create a mob, it should not be stored in a variable.
    /// </summary>

    public static Mob CreatePrototype(EMobPrototype mob_template)
    {
        Mob output = new Mob();
        output = mob_template switch
        {
            EMobPrototype.HUMAN => output
                .ChainName("Human")
                .ChainRace(ERace.HUMAN)
                .ChainJob(new(){Job.CreatePrototype(EJob.DEFAULT)}),
            _ => new Mob()
        };
        return output;
    }


    public Mob ChainName(string name)
    {
        DisplayedName = new(name);
        return this;
    }

    public Mob ChainMovementMode(EMovementMode mode)
    {
        MovementMode = mode;
        return this;
    }

    public Mob ChainFaction(EFaction faction)
    {
        Faction = faction;
        return this;
    }

    public Mob ChainState(EMobState state)
    {
        MobState = state;
        return this;
    }

    public Mob ChainEquipment(Item item)
    {
        Inventory.Slot? slot = MobInventory.GetSlotForItem(item, false);
        if (slot is null) return this;

        EventBus.InventoryItemAdded?.Invoke(MobInventory, slot, item);
        return this;
    }

    public Mob ChainJob(List<Job> jobs)
    {
        AddJob(jobs, true);
        return this;
    }

    public Mob ChainAction(Ability action)
    {
        AddAbility(action);
        return this;
    }

    public Mob ChainPosition(Vector3i position)
    {
        Position = position;
        return this;
    }

    public Mob ChainRace(ERace race)
    {
        Race = race;
        return this;
    }
}
