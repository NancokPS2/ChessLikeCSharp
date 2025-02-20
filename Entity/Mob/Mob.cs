using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Entity.Command;
using ChessLike.Extension;
using ChessLike.Shared;
using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
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

        //Default stats
        Stats = GetDefaultStats();

        //Default inventory
        Inventory inv = Inventory.FromResource(Inventory.LoadPreset(Inventory.EPreset.EQUIPMENT));
        MobInventory = inv;

        EventBusSetup();
    }

    #region Jobs
    public List<Job> GetJobs()
    {
        return Jobs;
    }

    private void UpdateJobs()
    {
        if (Jobs.Count == 0)
        {
            throw new Exception("No jobs defined.");
        }
        //Reset job modifiers
        Stats.BoostRemove(Job.BOOST_SOURCE);

        
        MobStatSet.StatBoost total_boost = new(Job.BOOST_SOURCE); 
        //TODO: Jobs should not be able to be null in the first place.
        foreach (Job job in Jobs.Where(x => x is not null))
        {
            //Average the stats from the job's.
            Stats.BoostAdd(job, false);

            //TODO: Make the selected mode be deterministic instead of selecting the last job of the list.
            SetMovementMode(job.MovementMode);
        }
        
        UpdateActions();
        Stats.SetToMax();
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
    #endregion

    #region Equipment
    public void EquipmentAdd(Item equip, Inventory.Slot slot)
    {
        var err = MobInventory.AddItem(equip, slot);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to equip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }
        
        Stats.BoostAdd(MobInventory, true);
        UpdateActions();
    }

    public void EquipmentAdd(Item item)
    {
        Inventory.Slot? slot = MobInventory.GetSlotForItem(item, true);
        
        EquipmentAdd(item, slot)
    }

    public void EquipmentRemove(Item equip)
    {
        var err = MobInventory.RemoveItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to unequip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }

        Stats.BoostAdd(MobInventory, true);
    }   
    #endregion

    #region Actions
        private Ability _movement = new();

    private void UpdateActions()
    {
        ClearAbility();

        foreach (IActionProvider job in Jobs)
        {
            AddAbility(job.GetAbilities());
        }

        foreach (IActionProvider item in MobInventory.GetItems())
        {
            AddAbility(item.GetAbilities());
        }
    }

    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMovementMode.WALK);
        AddAbility(_movement);
        _movement_mode = mode;
    }

    public void AddAbility(List<Ability> abilities) => abilities.ForEach(x => AddAbility(x));
    public void AddAbility(Ability action)
    {
        Actions.Add(action);
        action.Owner = this;
        action.OnAddedToMob();
    }


    public void ClearAbility()
    {
        List<EAbility> identifiers = new(from abil in Actions select abil.Identifier);
        foreach (var item in identifiers)
        {
            RemoveAbility(item);
        }
    }

    public void RemoveAbility(EAbility action_enum)
    {
        IEnumerable<Ability> to_remove = Actions.Where(x => x.Identifier == action_enum);
        foreach (var item in to_remove)
        {
            item.OnRemovedFromMob();
        }
        Actions.RemoveAll(x => x.Identifier == action_enum); 
    }


    public List<Ability> GetAbilities()
    {
        return Actions;
    }

    public void AddPassive(Passive passive)
    {
        Passives.Add(passive);
        passive.Owner = this;
        passive.OnAddedToMob();
    }

    public void RemovePassive(EPassive passive_enum)
    {
        IEnumerable<Passive> to_remove = Passives.Where(x => x.Identifier == passive_enum);
        foreach (var item in to_remove)
        {
            item.OnRemovedFromMob();
        }
        Passives.RemoveAll(x => x.Identifier == passive_enum); 
    }

    public void ClearPassive()
    {
        List<EPassive> identifiers = new(from abil in Passives select abil.Identifier);
        foreach (var item in identifiers)
        {
            RemovePassive(item);
        }
    }

    public List<Passive> GetPassives()
    {
        return Passives;
    }
    #endregion

    #region Misc
    public override string ToString()
    {
        string output = $"Name: {DisplayedName} \nFaction: {Faction} \nRace: {Race} \n";

        output += $"---\nJobs: {Jobs.ToStringList()}";

        return output;
    }

    public string ToStringStats() => $"---\nStats: {Stats}";

    public string ToStringActions()
    {
        string output = "";
        output += $"---\nAbilities: {Actions.ToStringList()}";
        output += $"---\nPassives: {Passives.ToStringList()}";
        return output;
    }
    #endregion
}