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

        SetupEventBus();
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

    #region Movement
    public void Move(Vector3i to)
    {
        Vector3i original_pos = Position;
        Position = to;
        EventBus.MobMoved?.Invoke(this, original_pos, Position);
    }

    public void MovePath(List<Vector3i> path)
    {
        foreach (var item in path)
        {
            Move(item);
        }
        EventBus.MobFinishedMoving?.Invoke(this, path);
    }

    #endregion

    #region Actions
    private Ability _movement = new();

    private void UpdateActions()
    {
        ClearAbility();

        foreach (IActionProvider job in Jobs)
        {
            AddAbility(job.GetActionEvents());
        }

        foreach (IActionProvider item in MobInventory.GetItems())
        {
            AddAbility(item.GetActionEvents());
        }
    }

    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMovementMode.WALK);
        AddAbility(_movement);
        _movement_mode = mode;
    }

    public void AddAbility(List<ActionEvent> actions) => actions.ForEach(x => AddAbility(x));
    public void AddAbility(ActionEvent action)
    {
        if (action is Ability abil) Actions.Add(abil);
        else if (action is Passive pas) Passives.Add(pas);
        
        action.Owner = this;
        action.OnAddedToMob();
        UpdateActions();
        EventBus.MobActionAdded?.Invoke(this, action);
    }

    public void RemoveAbility(ActionEvent action)
    {
        if (action is Ability abil) Actions.Remove(abil);
        else if (action is Passive pas) Passives.Remove(pas);

        action.OnRemovedFromMob();
        UpdateActions();
        EventBus.MobActionRemoved?.Invoke(this, action);
    }

    public void ClearAbility()
    {
        List<EAbility> identifiers = new(from abil in Actions select abil.Identifier);
        foreach (var item in Actions.Where(x => x is Ability))
        {
            RemoveAbility(item);
        }
    }

    public List<Ability> GetAbilities()
    {
        return Actions;
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