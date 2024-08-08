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
        //When a point is selected, mark a certain amount of cells based on the mode.
        //TODO to implement.
        public enum Mode
        {
            SINGLE, //Only the selected cell + AoE. Valid for most cases.
            STRAIGHT_LINE, //A line to the target point. Only works if the target coordinate shares at least 2 axis.
            CONE, //A cone towards the given position, similar to STRAIGHT_LINE
            PERPENDICULAR_LINE, //Line covering the front of the character + left and right.
        }
        public Mode mode = Mode.SINGLE;

        //Area when in SINGLE mode.
        public int AoESize = 0;

        //Distance from the user.
        public int RangeMax = 4;

        //Only works if there is a mob being hit that was deemed as valid.
        public bool NeedValidMob = true;

    }
}
