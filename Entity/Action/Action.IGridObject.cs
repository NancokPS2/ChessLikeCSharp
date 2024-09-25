using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Action : IGridObject
{
    public Vector3i GetPosition()
    {
        return Owner.GetPosition();
    }

    public bool IsValidMove(Grid grid, Vector3i from, Vector3i to)
    {
        return true;
    }

    public bool IsValidPositionToExist(Grid grid, Vector3i position)
    {
        Mob? mob_found = Global.ManagerMob.GetInPosition(position).First();
        bool mob_at_position = mob_found is Mob;

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
        uint max_range = TargetParams.TargetingRange;
        if (TargetParams.TargetingRangeStatBonus is StatName stat)
        {
            max_range += (uint)Owner.Stats.GetValue(stat);
        }
        if(position.DistanceManhattanTo(GetPosition()) > max_range){return false;}

        return true;
    }

    public int PathingGetHorizontalRange()
    {
        throw new NotImplementedException();
    }

    public int PathingGetVerticalRange()
    {
        throw new NotImplementedException();
    }

    public bool PathingIsInRange(Grid grid, Vector3i position)
    {
        throw new NotImplementedException();
    }
}
