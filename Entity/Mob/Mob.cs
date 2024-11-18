using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Entity;


public partial class Mob
{
    public string DisplayedName = "UNNAMED";
    private List<Job> Jobs = new(){Job.CreatePrototype(EJob.DEFAULT)};
    private List<Ability> Actions = new();
    private List<Passive> Passives = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public Inventory MobInventory = new();
    private EMovementMode _movement_mode;
    public EMovementMode MovementMode {set => SetMovementMode(value); get => _movement_mode;}
    public EMobState MobState = EMobState.BENCHED;
    public MobStatSet Stats = GetDefaultStats();

    public Vector3i Position;

    public Mob()
    {
        //TODO: Move this somewhere else
        Global.ManagerMob.AddPooled(this);
        Stats = GetDefaultStats();
        Inventory inv = Inventory.FromResource(Inventory.LoadPreset(Inventory.EPreset.EQUIPMENT));
        MobInventory = inv;
    }

    //TODO
    private Ability _movement = new();
    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = Ability.Create(EAbility.MOVE);
        AddAbility(_movement);
        _movement_mode = mode;
    }

    public void AddAbility(Ability action)
    {
        Actions.Add(action);
        action.Owner = this;
    }

    public void RemoveAbility(EAbility action_enum, bool all = true)
    {
        if (all)
        {
            Actions.RemoveAll(x => x.Identifier == action_enum);
        }
        else
        {
            Actions.Remove( Actions.First(x => x.Identifier == action_enum) );
        }

    }

    public List<Ability> GetAbilities()
    {
        return Actions;
    }

    public void AddPassive(Passive passive)
    {
        Passives.Add(passive);
        passive.Owner = this;
    }

    public void RemovePassive(EPassive passive_enum, bool all = true)
    {
        if (all)
        {
            Passives.RemoveAll(x => x.Identifier == passive_enum);
        }
        else
        {
            Passives.Remove( Passives.First(x => x.Identifier == passive_enum) );
        }

    }

    public List<Passive> GetPassives()
    {
        return Passives;
    }


    public void EquipmentAdd(Item equip)
    {
        var err = MobInventory.AddItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to equip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }
        
        Stats.BoostAdd(MobInventory);
    }

    public void EquipmentRemove(Item equip)
    {
        var err = MobInventory.RemoveItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to unequip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }

        Stats.BoostAdd(MobInventory);
    }

    private void UpdateJobs()
    {
        if (Jobs.Count == 0)
        {
            throw new Exception("No jobs defined.");
        }

        //Reset stats and update them.
        Stats = Jobs.First().Stats;

        foreach (Job job in Jobs)
        {
            //Average the stats from the job's.
            Stats = new(MobStatSet.GetAverage(Stats, job.Stats));

            //Add the actions.
            Actions.AddRange(job.Abilities);

            //TODO: Make the selected mode be deterministic instead of selecting the last job of the list.
            SetMovementMode(job.MovementMode);
        }
    }

    public static MobStatSet GetDefaultStats()
    {
        MobStatSet output = new();
        output.SetStat(StatName.HEALTH, 100);
        output.SetStat(StatName.ENERGY, 30);
        output.SetStat(StatName.AGILITY, 100);
        output.SetStat(StatName.STRENGTH, 100);
        output.SetStat(StatName.INTELLIGENCE, 100);
        output.SetStat(StatName.MOVEMENT, 3);
        output.SetStat(StatName.JUMP, 2);
        output.SetStat(StatName.DELAY, 100);
        return output;
    }

}