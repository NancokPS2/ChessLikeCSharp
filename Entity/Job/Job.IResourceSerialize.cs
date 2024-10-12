using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job : IResourceSerialize<Job, JobResource>
{
        public JobResource ToResource()
        {
            JobResource output = new();

            output.Identifier = Identifier;
            output.Stats = (MobStatSetResource)Stats.ToResource();
            output.Actions.AddRange(from action in Actions select action.ToResource());

            return output;
        }

        public static Job FromResource(JobResource resource)
        {
            Job output = new();

            output.Identifier = resource.Identifier;
            output.Stats = StatSet<StatName>.FromResource(resource.Stats);
            output.Actions.AddRange(from action in resource.Actions select Action.FromResource(action));

            return output;
        }
}
