using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using ChessLike.Entity.Relation;
using ChessLike.Shared;
using ChessLike.Turn;
using ChessLike.World;

namespace ChessLike.Entity;



public partial class Mob : IPosition, IRelation, IStats, ITurn
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
            Stats = StatSet.GetAverage(Stats, job.stats);

            //Add the actions.
            actions.AddRange(job.actions);
        }

    }
}