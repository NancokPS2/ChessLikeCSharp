using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using Godot;

namespace ChessLike.Entity.Action;

public partial class Ability
{
    //Properties used during the targeting phase to decide if the targets are valid.
    public class TargetingParameters
    {
        //Targeting
        //Distance from the user at which this can be used.
        public uint TargetingRange = 4;
        public StatName? TargetingRangeStatBonus = null;
        public int TargetingMaxPositions = 1;
        //Only works if the filter deems the position as valid.
        public bool TargetingNeedsValidMob = true;
        public bool TargetingUsesPathing = false;


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
        public uint AoERange = 0;
        public bool AoENeedsValidMob = false;

        public uint GetTotalRange(Mob owner)
        {
            uint output = TargetingRange;
            if (TargetingRangeStatBonus is StatName stat)
            {
                output += (uint)owner.Stats.GetValue(stat);
            }
            return output;
        }

        public List<Vector3i> GetTargetedPositions(UsageParameters usage_params)
        {
            //if (usage_params.PositionsTargeted.Count != 0 || usage_params.MobsTargeted.Count != 0){throw new Exception("This should be called BEFORE locations have been chosen.");}

            Vector3i origin = usage_params.OwnerRef.GetPosition();
            Grid grid = usage_params.GridRef;
            List<Vector3i> output = new();
            Mob owner = usage_params.OwnerRef;

            //If it uses pathing, just query that directly and move on.
            if (TargetingUsesPathing)
            {
                output = grid.NavGetPathablePositions(owner);
                return output;
            }

            uint max_range = GetTotalRange(owner);

            //Get general area for performance reasons.
            output = grid.GetShapeCube(origin, max_range);

            //Select positions within range.
            output = output.Where(x => x.DistanceManhattanTo(origin) <= max_range).ToList();

            return output;
        }

        public List<Vector3i> GetAoEPositions(UsageParameters usage_params, List<Vector3i> targets)
        {
            if (targets.Count == 0) {throw new Exception("No position to use AoE in.");}

            List<Vector3i> output = new();

            foreach (Vector3i item in targets)
            {    
                Vector3i origin = item;
                Grid grid = usage_params.GridRef;

                //Range is dictated by AoERange
                uint max_range = AoERange;

                List<Vector3i> cube = grid.GetShapeCube(origin, max_range);
                cube = cube.Where(x => x.DistanceManhattanTo(item) <= max_range).ToList();
                output.AddRange(cube);
            }

            if (output.Count == 0) {GD.PushWarning("Action's AoE is empty. Could not target here. Maybe tweak its TargetingParams.");}//throw new Exception("Nothing to select?");}

            return output;
        }

        public override string ToString()
        {
            /* Dictionary<string, string> output = new Dictionary<string, string>()
            {
                {"Targeting Range", TargetingRange.ToString()},

                {"AoE Range", AoERange.ToString()},

             };*/
            return this.GetFieldValuesAsDict<TargetingParameters>().ToStringList();
        }
    }
}
