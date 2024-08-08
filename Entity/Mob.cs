using ChessLike.Turn;

namespace ChessLike.Entity;



public partial class Mob : IPosition, IRelation, IStats, ITurn, IIdentify
{
    List<Job> jobs = new();
    List<Action> actions = new();

    public Mob()
    {
        Identity = new(Identity.INVALID_IDENTIFIER);
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
            Stats = StatSet.GetAverage(Stats, job.Stats);

            //Add the actions.
            actions.AddRange(job.actions);
        }

    }
}