namespace ChessLike.Entity;



public partial class Mob : IGridPosition, IGridReader, IRelation, IStats<StatName>, ITurn, IIdentify
{
    static Dictionary<Vector3i, Mob> MobToLocationDict = new();
    public List<Job> jobs = new();
    public List<Action> actions = new();
    public ERace race;
    public EFaction faction = EFaction.NEUTRAL;

    public Mob()
    {
        Vector3i vector = new();
        Position = vector;
        Identity = new("Unknown Mcnown");
    }
    
    public Mob(string name, List<Job> jobs)
    {
        Identity = new(name);
        this.jobs = jobs;
    }

    public void UpdateJobs()
    {
        //Reset stats and add update them.
        Stats = new();
        foreach (Job job in jobs)
        {
            //Average the stats from the job's.
            Stats = StatSet<StatName>.GetAverage(Stats, job.Stats);

            //Add the actions.
            actions.AddRange(job.actions);
        }
    }

    public static Mob? GetMobAtLocation(Vector3i location)
    {
        Mob? mob;
        MobToLocationDict.TryGetValue(location, out mob);
        return mob;
    }
}