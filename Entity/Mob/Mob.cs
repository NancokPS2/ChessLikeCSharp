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
    private List<ActionEvent> Actions = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public Inventory MobInventory = new();
    private EMovementMode movementMode;
    public EMovementMode MovementMode {set => SetMovementMode(value); get => movementMode;}
    private EMobState mobState = EMobState.BENCHED;
    public EMobState MobState
    {
        get => mobState;

        set
        {
            mobState = value;
            EventBus.MobStateChanged?.Invoke(this, mobState);
        }

    }

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

    public void AddJob(List<Job> jobs, bool replace)
    {
        if (replace) ClearJobs();

        foreach (var item in jobs)
        {
            Jobs.Add(item);
        }
        UpdateJobs();
    }

    public void AddJob(Job job) => AddJob(new List<Job>(){job}, false);

    public void RemoveJob(List<Job> jobs)
    {
        List<Job> to_delete = new(jobs);
        foreach (var item in to_delete)
        {
            Jobs.Remove(item);
        }
        UpdateJobs();
    }

    public void RemoveJob(Job job) => RemoveJob(new List<Job>(){job});

    private void ClearJobs()
    {
        RemoveJob(Jobs);
    }

    private void UpdateJobs()
    {
        //Reset job modifiers
        Stats.BoostRemove(Job.BOOST_SOURCE);

        MobStatSet.StatBoost total_boost = new(Job.BOOST_SOURCE); 

        //TODO: Jobs should not be able to be null in the first place.
        foreach (Job job in Jobs)//.Where(x => x is not null))
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
        ClearAction();

        foreach (IActionProvider job in Jobs)
        {
            AddAction(job.GetActionEvents());
        }

        foreach (IActionProvider item in MobInventory.GetItems())
        {
            AddAction(item.GetActionEvents());
        }
    }

    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMovementMode.WALK);
        AddAction(_movement);
        movementMode = mode;
    }

    public void AddAction(ActionEvent action) => AddAction(new List<ActionEvent>(){action});

    public void AddAction(List<ActionEvent> actions)
    {
        foreach (var action in actions)
        {
            Actions.Add(action);
            action.Owner = this;
            EventBus.MobActionAdded?.Invoke(this, action);
        }
    }

    public void RemoveAction(ActionEvent action) => RemoveAction(new List<ActionEvent>(){action});

    public void RemoveAction(List<ActionEvent> actions)
    {
        List<ActionEvent> to_delete = new(actions);
        foreach (var item in to_delete)
        {
            Actions.Remove(item);
            EventBus.MobActionRemoved?.Invoke(this, item);
        }

    }

    public void ClearAction()
    {
        RemoveAction(Actions);
    }

    public List<Ability> GetAbilities()
    {
        List<Ability> output = new();
        foreach (var item in Actions)
        {
            if (item is Ability abil)
            {
                output.Add(abil);
            }
        }
        return output;
    }

    public List<Passive> GetPassives()
    {
        List<Passive> output = new();
        foreach (var item in Actions)
        {
            if (item is Passive pas)
            {
                output.Add(pas);
            }
        }
        return output;
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
        output += $"---\nAbilities: {GetAbilities().ToStringList()}";
        output += $"---\nPassives: {GetPassives().ToStringList()}";
        return output;
    }
    #endregion
}