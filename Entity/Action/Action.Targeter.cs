using ChessLike.World;

namespace ChessLike.Entity;

public partial class Action
{
    public static class Targeter
    {

        /// <summary>
        /// Used to check which positions are selectable when using an action.
        /// </summary>
        /// <param name="origin">From where to start checking, usually where the user is.</param>
        /// <param name="usage_params">A pre-created UsageParams object. With an owner, grid and action_reference set.</param>
        /// <returns></returns>
        public static List<Vector3i> GetSelectableCells(UsageParams usage_params)
        {
            Action action = usage_params.action_reference;
            Grid grid = usage_params.grid;
            Vector3i origin = usage_params.owner.Position;
            TargetingParams.AoEMode aoe_mode = action.TargetParams.AoeShape;

            //Range is forced to 1 when using anything other than SINGLE mode.
            int targeting_range = action.TargetParams.AoeShape != TargetingParams.AoEMode.SINGLE ? 
                1 : action.TargetParams.TargetingRange;

            //Filters.
            List<Func<Vector3i, bool>> filters = new();

            //Regarding vacancy of the position.
            switch (action.TargetParams.NeededVacancy)
            {
                case TargetingParams.VacancyStatus.HAS_MOB:
                    //filters.Add(Filter.HasMob);
                    filters.Add(x => Global.ManagerMob.GetInPosition(x) != null);
                    break;

                case TargetingParams.VacancyStatus.HAS_NO_MOB:
                    filters.Add(x => Global.ManagerMob.GetInPosition(x) == null);
                    break;

                default:
                    break;
            }

            //Regarding flood fill mode
            switch (action.TargetParams.TargetingFloodFillMode)
            {
                case TargetingParams.FloodFillMode.AVOID_SOLIDS:
                    filters.Add( x => !grid.IsFlagInPosition(x, CellFlag.SOLID));
                    break;

                default:
                    break;
            }

            List<Vector3i> output = grid.GetFloodFill(origin, targeting_range, filters);

            return output;
            
            //return GetSelectableCells(grid, action, origin, aoe_mode);
        }

/*         public static List<Vector3i> GetSelectableCells(Grid grid, Action action, Vector3i origin, TargetingParams.AoEMode aoe_mode)
        {
        } */

        public static List<Vector3i> GetTargetedAoE(Vector3i target, UsageParams usage_params)
        {
            List<Vector3i> output = new();

            Action action = usage_params.action_reference;

            switch (action.TargetParams.AoeShape)
            {

                case TargetingParams.AoEMode.SINGLE:
                    output.Add(target);
                    break;

                case TargetingParams.AoEMode.STRAIGHT_LINE:
                    output.Add(target);
                    break;

                case TargetingParams.AoEMode.CONE:
                    output.Add(target);
                    break;

                default: throw new ArgumentException();
            };
            return output;
        }
    }
    
}
