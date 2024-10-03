using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

public partial class Mob
{
    /// <summary>
    /// This class should only be used to create a mob, it should not be stored in a variable.
    /// </summary>

    public static Mob CreatePrototype(EMobPrototype mob_template)
    {
        Mob output = new();
        output = mob_template switch
        {
            EMobPrototype.HUMAN => output
                .ChainName("Human")
                .ChainJob(Global.ManagerJob.GetFromEnum(EJob.DEFAULT))
                .ChainAction(Global.ManagerAction.GetFromEnum(EAction.MOVE))
                .ChainRace(ERace.HUMAN),
            _ => new Mob()
        };
        return output;
    }

    public Mob ChainBaseStats()
    {
        Stats.SetStat(StatName.HEALTH, 100);
        Stats.SetStat(StatName.ENERGY, 30);
        Stats.SetStat(StatName.AGILITY, 100);
        Stats.SetStat(StatName.STRENGTH, 100);
        Stats.SetStat(StatName.INTELLIGENCE, 100);
        Stats.SetStat(StatName.MOVEMENT, 3);
        if (Stats.GetValue(StatName.MOVEMENT) != 3)
        {
            throw new Exception("What!?");
        }
        Stats.SetStat(StatName.JUMP, 2);
        return this;
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

    public Mob ChainEquipment(Equipment item)
    {
        EquipmentAdd(item);
        return this;
    }

    public Mob ChainJob(List<Job> jobs)
    {
        Jobs.Clear();

        foreach (Job job in jobs)
        {
            Jobs.Add(job);
        }
        UpdateJobs();
        return this;
    }

    public Mob ChainJob(Job job)
    {
        Jobs.Clear();
        Jobs.Add(job);
        UpdateJobs();
        return this;
    }

    public Mob ChainAction(Action action)
    {
        AddAction(action);
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
