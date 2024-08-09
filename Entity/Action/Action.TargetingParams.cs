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
        public enum FloodFillMode
        {
            NONE,
            AVOID_SOLIDS,
        }
        public enum VacancyStatus
        {
            HAS_MOB,
            HAS_NO_MOB,
        }

        //Targeting
        //Distance from the user at which this can be used.
        public int TargetingRange = 4;
        //When choosing which cells are selectable for targeting, use flood fill.
        public FloodFillMode TargetingFloodFillMode = FloodFillMode.NONE;
        //Only works if there is a mob being hit that was deemed as valid.
        public VacancyStatus NeededVacancy = VacancyStatus.HAS_NO_MOB;


        //AoE
        public enum AoEMode
        {
            SINGLE, //Only the selected cell + AoE. Valid for most cases.
            STRAIGHT_LINE, //A line to the target point. Only works if the target coordinate shares at least 2 axis. RangeMax is treated as 1. Size is controlled by AoESize.
            CONE, //A cone towards the given position, similar to STRAIGHT_LINE. RangeMax is treated as 1. Size is controlled by AoESize.
            PERPENDICULAR_LINE, //Line covering the front of the character + left and right. RangeMax is treated as 1.
        }
        public AoEMode AoeShape = AoEMode.SINGLE;
        //Area when in SINGLE mode.
        public int AoERange = 0;
        //When getting the AoE allowed, use a flood fill approach.
        public FloodFillMode AoEFloodFillMode = FloodFillMode.NONE;


    }
}
