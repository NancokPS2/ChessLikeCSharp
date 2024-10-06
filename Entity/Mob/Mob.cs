using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Entity;


public partial class Mob : IStats<StatName>
{
    static Dictionary<Vector3i, Mob> MobToLocationDict = new();
    public string DisplayedName = "UNNAMED";
    public List<Job> Jobs = new(){Job.CreatePrototype(EJob.DEFAULT)};
    private List<Action> Actions = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public MobInventory Inventory = new();
    public EMovementMode MovementMode = EMovementMode.WALK;
    public EMobState MobState = EMobState.BENCHED;
    public StatSet<StatName> Stats { get; set; } = new(){
    };

    public Vector3i Position;

    public Mob()
    {
        DisplayedName = new("Unknown Mcnown");
        //TODO: Move this somewhere else
        Global.ManagerMob.Add(this);
    }

    //TODO
    private Action _movement;
    public void SetMovementMode(EMovementMode mode)
    {
        Actions.Remove(_movement);

        Global.ManagerAction.GetFromEnum(EAction.MOVE);
    }

    public void AddAction(Action action)
    {
        Actions.Add(action);
        action.Owner = this;
    }

    public void RemoveAction(EAction action_enum, bool all = true)
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

    public List<Action> GetActions()
    {
        return Actions;
    }

    public void EquipmentAdd(Equipment equip)
    {
        var err = Inventory.AddItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to equip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }
        
        Stats.BoostAdd(Inventory);
    }

    public void EquipmentRemove(Equipment equip)
    {
        var err = Inventory.RemoveItem(equip);
        if (err != Shared.Storage.Inventory.Error.NONE)
        {
            GD.PushWarning(string.Format("Failed to unequip {0} due to {1}.", new object[]{ equip.Name, err.ToString()}));
        }

        Stats.BoostAdd(Inventory);
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
            Stats = StatSet<StatName>.GetAverage(Stats, job.Stats);

            //Add the actions.
            Actions.AddRange(job.Actions);
        }
    }


}