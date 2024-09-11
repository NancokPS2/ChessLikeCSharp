using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

public partial class GridPathing
{
    private Grid grid;
    private Dictionary<EMovementMode, AStar3D> AStarGrids = new();
    private Dictionary<AStar3D, Dictionary<Vector3i, long>> AStarIdToPosition = new();

    public GridPathing(Grid grid)
    {
        this.grid = grid;
    }

    public void GeneratePathing()
    {
        AStarGrids.Clear();

        foreach (var move_mode in Enum.GetValues<EMovementMode>())
        {
            AStar3D aStar = new();
            AStarGrids[move_mode] = aStar;
            AStarIdToPosition[aStar] = new();

            foreach (var position in grid.GetUsedPositions())
            {
                if (IsPositionPathable(position, move_mode))
                {
                    long id = aStar.GetAvailablePointId();
                    aStar.AddPoint(
                        id,
                        position.ToGVector3()
                        );

                    AStarIdToPosition[aStar][position] = id;
                    
                }
            }
            //TODO: Connection
            foreach (var item in AStarIdToPosition[aStar].Keys)
            {
                
            }
        }
    }

    public bool IsPositionPathable(Vector3i position, EMovementMode mode)
    {
        bool solid_below = grid.IsFlagInPosition(position + Vector3i.DOWN, CellFlag.SOLID);
        bool air_here = grid.IsFlagInPosition(position, CellFlag.AIR);
        bool liquid_here = grid.IsFlagInPosition(position, CellFlag.LIQUID);
        switch (mode)
        {
            case EMovementMode.WALK:
                return solid_below && air_here;
            
            case EMovementMode.FLY:
                return air_here;
            
            case EMovementMode.SWIM:
                return liquid_here;

            default:
                throw new NotImplementedException();
        }
    }

    public bool IsPositionConnectable(Vector3i from, Vector3i to)
    {
        if (from.DistanceManhattanTo(to) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
