using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Action
{
    public List<Vector3i> TargetingGetPositionsInRange(UsageParams usage_params)
    {
        //if (usage_params.PositionsTargeted.Count != 0 || usage_params.MobsTargeted.Count != 0){throw new Exception("This should be called BEFORE locations have been chosen.");}

        Vector3i origin = usage_params.OwnerRef.GetPosition();
        Grid grid = usage_params.GridRef;
        List<Vector3i> output = new();
        int max_range = TargetParams.GetTotalRange(Owner);

        //Get general area for performance reasons.
        output = grid.GetShapeCube(origin, max_range);

        //Validate positions
        output = output.Where( x => TargetingIsValidTargetPosition(usage_params, x)).ToList();
        output = output.Where( x => grid.IsPositionInbounds(x)).ToList();

        return output;
    }

    public List<Vector3i> TargetingGetPositionsInAoE(UsageParams usage_params, Vector3i position)
    {
        Vector3i origin = position;
        Grid grid = usage_params.GridRef;
        List<Vector3i> output = new();
        int max_range = TargetParams.GetTotalRange(Owner);

        output = grid.GetShapeCube(origin, max_range);

        output = output.Where( x => TargetingIsValidTargetPosition(usage_params, x)).ToList();

        return output;
    }

    public bool TargetingIsValidTargetPosition(UsageParams usage_params, Vector3i position)
    {
        List<Mob>? mobs_found = Global.ManagerMob.GetInPosition(position);
        bool mob_at_position = mobs_found.Count > 0 && mobs_found is Mob;

        //Vacancy status
        switch (TargetParams.NeededVacancy)
        {
            case TargetingParameters.VacancyStatus.HAS_MOB:
                if(!mob_at_position){return false;}
                break;

            case TargetingParameters.VacancyStatus.HAS_NO_MOB:
                if(mob_at_position){return false;}
                break;

            default:
                throw new NotImplementedException();
        }
        
        //Targeting range check
        int max_range = TargetParams.GetTotalRange(Owner);
        if(position.DistanceManhattanTo(usage_params.OwnerRef.GetPosition()) > max_range){return false;}

        return true;
    }
    
}
