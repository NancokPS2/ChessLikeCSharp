using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using Godot;

namespace ChessLike.Entity.Action;
//Properties used during the targeting phase to decide if the targets are valid.
public partial class TargetingParameters : Resource
{
    //Targeting
    //Distance from the user at which this can be used.
    [Export]
    public uint TargetingRange = 4;

    [Export]
    public EStatName TargetingRangeStatBonus = EStatName.NONE;

    [Export]
    public int TargetingMaxPositions = 1;

    [Export]
    //Only works if the filter deems the position as valid.
    public bool TargetingNeedsValidMob = true;

    [Export]
    public bool TargetingUsesPathing = false;

    //AoE
    public enum AoEMode
    {
        SINGLE, //Only the selected cell + AoE. Valid for most cases.
        STRAIGHT_LINE, //A line to the target point. Only works if the target coordinate shares at least 2 axis. RangeMax is treated as 1. Size is controlled by AoESize.
        CONE, //A cone towards the given position, similar to STRAIGHT_LINE. RangeMax is treated as 1. Size is controlled by AoESize.
        PERPENDICULAR_LINE, //Line covering the front of the character + left and right. RangeMax is treated as 1.
    }
    [Export]
    public AoEMode AoEShape = AoEMode.SINGLE;

    [Export]
    //Area when in SINGLE mode.
    public uint AoESize = 0;

    public List<Vector3i> GetTargetingShape(Vector3i.Rotation direction)
    {
        List<Vector3i> output = new();

        switch (AoEShape)
        {
            case AoEMode.SINGLE:
                output.Append(Vector3i.ZERO);
                break;

            case AoEMode.STRAIGHT_LINE:
                for (int i = 0; i < AoESize; i++)
                {
                    output.Append(Vector3i.FORWARD * i);
                }
                break;

            case AoEMode.PERPENDICULAR_LINE:
                output.Append(Vector3i.ZERO);

                //Skip if the size is 0
                if (AoESize == 0) break;

                for (int distance = 1; distance <= AoESize; distance++)
                {
                    output.Append(Vector3i.LEFT * (distance + 1));
                    output.Append(Vector3i.RIGHT * (distance + 1));
                }
                break;

            default: throw new Exception($"Invalid or unimplemented mode ({AoEShape})");
        }

        List<Vector3i> rotatedOutput = new();
        foreach (var item in output)
        {
            rotatedOutput.Append( item.Rotated(direction) );
        }

        return rotatedOutput;
    }

    public uint GetTotalRange(Mob owner)
    {
        uint output = TargetingRange;
        if (TargetingRangeStatBonus is EStatName stat)
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
            uint max_range = AoESize;

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

