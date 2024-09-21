using ChessLike.Turn;

namespace ChessLike.Entity;


public partial class Mob : IStats<StatName>
{
    static Dictionary<Vector3i, Mob> MobToLocationDict = new();
    public string DisplayedName = "UNNAMED";
    public List<Job> Jobs = new();
    private List<Action> Actions = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public Inventory Inventory = new(5);
    public EMovementMode MovementMode = EMovementMode.WALK;
    public EMobState MobState = EMobState.BENCHED;

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
    public List<Action> GetActions()
    {
        return Actions;
    }

    private void UpdateJobs()
    {
        //Reset stats and add update them.
        Stats = new();
        foreach (Job job in Jobs)
        {
            //Average the stats from the job's.
            Stats = StatSet<StatName>.GetAverage(Stats, job.Stats);

            //Add the actions.
            Actions.AddRange(job.actions);
        }
    }


}