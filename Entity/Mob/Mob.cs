namespace ChessLike.Entity;


public partial class Mob : IGridPosition, IStats<StatName>, ITurn, IEnumIdentifier<EMob>
{
    static Dictionary<Vector3i, Mob> MobToLocationDict = new();
    public string DisplayedName = "UNNAMED";
    public List<Job> Jobs = new();
    public List<Action> Actions = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public Inventory Inventory = new(5);
    public EMovementMode MovementMode = EMovementMode.WALK;

    public EMob Identifier { get; set; }


    public Mob()
    {
        DisplayedName = new("Unknown Mcnown");
        //TODO: Move this somewhere else
        Global.ManagerMob.Add(this);
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