using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    /// <summary>
    /// This class should only be used to create a mob, it should not be stored in a variable.
    /// </summary>

    public Mob ChainIdentity(string identifier, string name, bool concealed = false)
    {
        Identity = new(identifier, name, concealed);
        return this;
    }


    public Mob ChainJob(List<Job> jobs)
    {
        Jobs.Clear();

        foreach (Job job in jobs)
        {
            Jobs.Add(job);
        }
        return this;
    }

    public Mob ChainJob(Job job)
    {
        Jobs.Clear();
        Jobs.Add(job);
        return this;
    }

    public Mob ChainPosition(Vector3i position)
    {
        Position = position;
        return this;
    }

    /// <summary>
    /// Should be ran after the rest of the methods.
    /// </summary>
    /// <returns></returns>
    public Mob ChainUpdate()
    {
        UpdateJobs();
        
        return this;
    }
    public Mob ChainResult()
    {
        return ChainUpdate();
    }
}
