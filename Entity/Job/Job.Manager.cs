using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Resolvers;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;


public partial class Job
{
    public class Manager : Manager<Job>
    {

        public List<Job> CreatePrototypes()
        {
            List<Job> output = new();
            foreach (var item in Enum.GetValues<EJob>())
            {
                output.Add(Job.CreatePrototype(item));
            }

            return output;
        }
    }
}
