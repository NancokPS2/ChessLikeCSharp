using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;

namespace ChessLike.Entity;

public partial class Job : IResourceSerialize<Job, JobResource>
{
        public JobResource ToResource()
        {
            JobResource output = new();

            output.Identifier = Identifier;
            output.StatMultiplicativeBoostDict = new(StatMultiplicativeBoostDict);
            output.Actions.AddRange(from action in Abilities select action.ToResource());

            return output;
        }

        public static Job FromResource(JobResource resource)
        {
            Job output = new();

            output.Identifier = resource.Identifier;
            Godot.Collections.Dictionary<StatName, float> from = resource.StatMultiplicativeBoostDict;
            output.StatMultiplicativeBoostDict = from.ToDictionary(
                x => x.Key, y => y.Value
                );
            
            output.Abilities.AddRange(from action in resource.Actions select Ability.FromResource(action));

            return output;
        }
}
