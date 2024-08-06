using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    //Properties used during the targeting phase to decide if the targets are valid.
    public class TargetingParams
    {
        //Distance from the user.
        public float RangeMax = 4;

        //Only works if there is a mob being hit that was deemed as valid.
        public bool NeedValidMob = true;

    }
}
