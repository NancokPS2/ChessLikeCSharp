using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using Godot;

namespace ChessLike.Entity;

public partial class Action
{
    public List<Vector3i> TargetingGetPositionsInRange(UsageParams usage_params)
    {
        //if (usage_params.PositionsTargeted.Count != 0 || usage_params.MobsTargeted.Count != 0){throw new Exception("This should be called BEFORE locations have been chosen.");}

        Vector3i origin = usage_params.OwnerRef.GetPosition();
        Grid grid = usage_params.GridRef;
        List<Vector3i> output = new();

        //If it uses pathing, just query that directly and move on.
        if (TargetParams.RespectsOwnerPathing)
        {
            output = grid.NavGetPathablePositions(Owner);
            return output;
        }

        uint max_range = TargetParams.GetTotalRange(Owner);

        //Get general area for performance reasons.
        output = grid.GetShapeCube(origin, max_range);

        //Validate positions
        output = output.Where( x => TargetingIsValidTargetPosition(usage_params, x, false)).ToList();

        return output;
    }

    public List<Vector3i> TargetingGetPositionsInAoE(UsageParams usage_params)
    {
        if (usage_params.PositionsTargeted.Count == 0) {throw new Exception("No position to use AoE in.");}

        List<Vector3i> output = new();

        foreach (var item in usage_params.PositionsTargeted)
        {    
            Vector3i origin = item;
            Grid grid = usage_params.GridRef;

            //Range is dictated by AoERange
            uint max_range = TargetParams.AoERange;

            output = grid.GetShapeCube(origin, max_range);
        }

        output = output.Where( x => TargetingIsValidTargetPosition(usage_params, x, true)).ToList();
        if (output.Count == 0) {GD.PushWarning("Action's AoE is empty. Could not target here. Maybe tweak its TargetingParams.");}//throw new Exception("Nothing to select?");}

        return output;
    }

    public bool TargetingIsValidTargetPosition(UsageParams usage_params, Vector3i position, bool AoE)
    {
        List<Mob>? mobs_found = Global.ManagerMob.GetInPosition(position);
        bool mob_at_position = mobs_found.Count > 0 && mobs_found is Mob;

        //Vacancy status
        switch (TargetParams.NeededVacancy)
        {
            case TargetingParameters.VacancyStatus.HAS_MOB:
                if(!mob_at_position)
                {
                    return false;
                }
                break;

            case TargetingParameters.VacancyStatus.HAS_NO_MOB:
                if(mob_at_position)
                {
                    return false;
                }
                break;

            default:
                throw new NotImplementedException();
        }
        
        //Targeting range check
        if (AoE)
        {
            foreach (var item in usage_params.PositionsTargeted)
            {
                uint max_range = TargetParams.AoERange;
                if(position.DistanceManhattanTo(item) > max_range)
                {
                    return false;
                }    
                
            }
        }
        else
        {
            uint max_range = TargetParams.GetTotalRange(Owner);
            if(position.DistanceManhattanTo(usage_params.OwnerRef.GetPosition()) > max_range)
            {
                return false;
            }    
        }

        return true;
    }
    
}
