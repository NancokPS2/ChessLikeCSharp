using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Resolvers;
using ChessLike.Shared.Serialization;

namespace ChessLike.Entity;


public partial class Job
{
    public class Manager : SerializableManager<Job>
    {


        public override List<Job> CreatePrototypes()
        {
            List<Job> output = new()
                {
                    Job.Create(EJob.CIVILIAN),
                    Job.Create(EJob.WARRIOR),
                    Job.Create(EJob.WIZARD),
                };
            return output;
        }
        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "job"
            );

        }

        public Job GetFromEnum(EJob job)
        {
            return GetAll().First(x => x.Identifier == job);
        }
    }
}
