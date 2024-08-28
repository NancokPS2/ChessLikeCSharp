using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Job
{
    public class Loader : ILoadsByIdentity<EJob, Job>
    {
        public Job GetNewObject(EJob identity_enum)
        {
            Job output = new();
            output = identity_enum switch
            {
                EJob.WARRIOR => output.ChainDefaultStats().ChainWarrior(),
                EJob.WIZARD => output.ChainDefaultStats().ChainWizard(),
                EJob.RANGER => output.ChainDefaultStats(),
                _ => output.ChainDefaultStats(),
            };
            return output;
        }
    }



}
