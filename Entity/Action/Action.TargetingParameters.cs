using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    //Properties used during the targeting phase to decide if the targets are valid.
    public class TargetingParameters
    {
        public enum VacancyStatus
        {
            HAS_MOB,
            HAS_NO_MOB,
        }

        //Targeting
        //Distance from the user at which this can be used.
        public int TargetingRange = 4;
        public StatName? TargetingRangeStatBonus;
        //Only works if there is a mob being hit that was deemed as valid.
        public VacancyStatus NeededVacancy = VacancyStatus.HAS_MOB;
        public bool RespectsOwnerPathing = false;


        //AoE
        public enum AoEMode
        {
            SINGLE, //Only the selected cell + AoE. Valid for most cases.
            STRAIGHT_LINE, //A line to the target point. Only works if the target coordinate shares at least 2 axis. RangeMax is treated as 1. Size is controlled by AoESize.
            CONE, //A cone towards the given position, similar to STRAIGHT_LINE. RangeMax is treated as 1. Size is controlled by AoESize.
            PERPENDICULAR_LINE, //Line covering the front of the character + left and right. RangeMax is treated as 1.
            FLOOD_FILL_NO_SOLID
        }
        public AoEMode AoeShape = AoEMode.SINGLE;
        //Area when in SINGLE mode.
        public int AoERange = 0;

        public int GetTotalRange(Mob owner)
        {
            int output = TargetingRange;
            if (TargetingRangeStatBonus is StatName stat)
            {
                output += (int)owner.Stats.GetValue(stat);
            }
            return output;
        }
    }
}
