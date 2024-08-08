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
    public class Builder
    {
        private Mob mob;
        public Builder()
        {
            mob = new();
        }
        public Builder(Mob mob)
        {
            this.mob = mob;
        }

        public Builder SetIdentity(string identifier, string name, bool concealed = false)
        {
            mob.Identity = new(identifier, name, concealed);
            return this;
        }

        public Builder SetJob(Job[] jobs)
        {
            mob.jobs.Clear();

            foreach (Job job in jobs)
            {
                mob.jobs.Add(job);
            }
            return this;
        }

        public Builder SetJob(Job job)
        {
            mob.jobs.Clear();
            mob.jobs.Add(job);
            return this;
        }

        public Builder SetPosition(Vector3i position)
        {
            mob.GridPosition = position;
            return this;
        }

        /// <summary>
        /// Should be ran after the rest of the methods.
        /// </summary>
        /// <returns></returns>
        public Builder Update()
        {
            mob.UpdateJobs();
            
            return this;
        }
        public Mob Result()
        {
            return mob;
        }


    }
}
