using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using ChessLike.Shared;
using ChessLike.Storage;
using ChessLike.Turn;
using ChessLike.World;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class Mob : Resource
{
    private Grid CurrentGrid;

    [Export]
    public string DisplayedName = "UNNAMED";
    [Export]
    private Godot.Collections.Array<Job> jobs
    {
        set => Jobs = new(value);
        get => new(Jobs);
    }
    private List<Job> Jobs = new() { Job.CreatePrototype(EJob.DEFAULT) };

    [Export]
    private Godot.Collections.Array<ActionEvent> actions
    {
        set => Actions = new(value);
        get => new(Actions);
    }
    private List<ActionEvent> Actions = new();

    [Export]
    public ERace Race = ERace.HUMAN;

    [Export]
    public EFaction Faction = EFaction.NEUTRAL;

    public EMobMovementMode MovementMode { set => SetMovementMode(value); get => movementMode; }
    private EMobMovementMode movementMode;

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

    public bool IsInCombat() => mobState == EMobState.COMBAT;

    public MobStatSet Stats = GetDefaultStats();

    private Vector3i Position;

    public Mob()
    {
        //TODO: Move this somewhere else
        Global.ManagerMob.AddPooled(this);

        //Default stats
        Stats = GetDefaultStats();

        SetupEventBus();
    }

    #region Inventory
    [Export]
    private MobInventory EquipmentInventory = new();

    public void UpdateEquipmentStatBoosts()
    {
        MobStatBoost outputStatBoost = new(ItemEquipment.BOOST_SOURCE);

        foreach (Item item in EquipmentInventory.GetItems())
        {
            ItemEquipment equipment;

            //Make sure it is equipment
            if (item is ItemEquipment _equip) equipment = _equip;
            else throw new Exception($"This inventory is for equipment only. Found {item}");

            outputStatBoost = (MobStatBoost)(outputStatBoost + equipment.StatBoost);
        }

        Stats.BoostAdd(outputStatBoost, true);
    }

    public void UpdateJobStatBoosts()
    {
        MobStatBoost outputBoost = new(Job.BOOST_SOURCE);

        //TODO: Jobs should not be able to be null in the first place.
        foreach (Job job in Jobs)//.Where(x => x is not null))
        {
            //Average the stats from the job's.
            outputBoost = outputBoost + job.GetStatBoost();
        }

        Stats.BoostAdd(outputBoost, true);
    }

    #endregion

    #region Faction
    public Faction GetFaction()
        => Global.ManagerFaction.GetPooledByEnum(Faction);

    #endregion

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

    public void AddJob(Job job) => AddJob(new List<Job>() { job }, false);

    public void RemoveJob(List<Job> jobs)
    {
        List<Job> to_delete = new(jobs);
        foreach (var item in to_delete)
        {
            Jobs.Remove(item);
        }
        UpdateJobs();
    }

    public void RemoveJob(Job job) => RemoveJob(new List<Job>() { job });

    private void ClearJobs()
    {
        RemoveJob(Jobs);
    }

    private void UpdateJobs()
    {

        //TODO: Jobs should not be able to be null in the first place.
        foreach (Job job in Jobs)//.Where(x => x is not null))
        {
            //TODO: Make the selected mode be deterministic instead of selecting the last job of the list.
            SetMovementMode(job.MovementMode);
        }
        //Reset job modifiers
        UpdateJobStatBoosts();

        UpdateActions();
        Stats.SetToMax();
    }

    public static MobStatSet GetDefaultStats()
    {
        MobStatSet output = new();
        output.SetStat(EStatName.HEALTH, 100);
        output.SetStat(EStatName.ENERGY, 30);
        output.SetStat(EStatName.AGILITY, 100);
        output.SetStat(EStatName.STRENGTH, 100);
        output.SetStat(EStatName.INTELLIGENCE, 100);
        output.SetStat(EStatName.MOVEMENT, 3);
        output.SetStat(EStatName.JUMP, 2);
        output.SetStat(EStatName.DELAY, 100);
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

    public void MoveRelative(Vector3i to)
    {
        Move(GetPosition() + to);
    }

    public void MoveTroughPath(List<Vector3i> path)
    {
        foreach (var item in path)
        {
            Move(item);
        }
        EventBus.MobFinishedPathMove?.Invoke(this, path);
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

        foreach (IActionProvider item in EquipmentInventory.GetItems())
        {
            AddAction(item.GetActionEvents());
        }
    }

    public void SetMovementMode(EMobMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMobMovementMode.WALK);
        AddAction(_movement);
        movementMode = mode;
    }

    public void AddAction(ActionEvent action) => AddAction(new List<ActionEvent>() { action });

    public void AddAction(List<ActionEvent> actions)
    {
        foreach (var action in actions)
        {
            Actions.Add(action);
            action.Owner = this;
            EventBus.MobActionAdded?.Invoke(this, action);
        }
    }

    public void RemoveAction(ActionEvent action) => RemoveAction(new List<ActionEvent>() { action });

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

    public List<Ability> GetPassives()
        => GetAbilities().Where(x => x.IsPassive()).ToList();
    #endregion

    #region Per Turn Values
    protected int TurnActionsUsed;
    protected int TurnReactionsUsed;
    protected bool TurnActive;
    
    protected int GetActionsUsed() => TurnActionsUsed;
    protected int GetReactionsUsed() => TurnReactionsUsed;

    public bool HasActionUsesLeft() => GetActionsUsed() < 1;
    public bool HasReactionUsesLeft() => GetReactionsUsed() < int.MaxValue;

    protected void TurnResourceReset()
    {
        TurnActionsUsed = 0;
        TurnReactionsUsed = 0;
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
        //output += $"---\nPassives: {GetPassives().ToStringList()}";
        return output;
    }
    #endregion
}