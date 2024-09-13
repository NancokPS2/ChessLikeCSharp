using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
using Vector3 = System.Numerics.Vector3;

namespace ChessLike.World;

public partial class Grid
{

public partial class AStarGridPathing
{
    public MeshInstance3D DebugMesh = new();
    private Grid grid;
    private Dictionary<EMovementMode, AStar3D> AStarGrids = new();
    private Dictionary<AStar3D, Dictionary<Vector3i, long>> AStarPositionToId = new();

    public AStarGridPathing(Grid grid)
    {
        this.grid = grid;
    }

    public void PathingBake()
    {
        AStarGrids.Clear();

        foreach (var move_mode in Enum.GetValues<EMovementMode>())
        {
            AStar3D aStar = new();
            AStarGrids[move_mode] = aStar;
            AStarPositionToId[aStar] = new();

            foreach (var position in grid.GetUsedPositions())
            {
                if (IsPositionPathable(position, move_mode))
                {
                    long id = aStar.GetAvailablePointId();
                    aStar.AddPoint(
                        id,
                        position.ToGVector3()
                        );

                    AStarPositionToId[aStar][position] = id;
                    
                }
            }
            //TODO: Connection
            foreach (var point in aStar.GetPointIds())
            {
                Vector3i initial_pos = new (aStar.GetPointPosition(point));
                int count = 0;
                foreach (var point_target in aStar.GetPointIds())
                {
                    Vector3i final_point = new (aStar.GetPointPosition(point_target));
                    if (initial_pos.DistanceManhattanWithToleranceTo(final_point, new(0,grid.Boundary.Y,0)) == 1)
                    {
                        aStar.ConnectPoints(point, point_target);
                        count ++;
                    }
                    if (count == 4)
                    {
                        break;
                    }
                }
               /*  foreach (Vector3i direction in Vector3i.DIRECTIONS)
                {
                    Vector3i valid_pos;
                    foreach (Vector3i column_pos in grid.GetUsedPositionsInThisColumn(vector + direction))
                    {
                        if (IsPositionPathable(column_pos, move_mode))
                        {
                            valid_pos = column_pos;
                        }
                    } 
                } */
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

    public void DrawPointsToMesh(AStar3D aStar, ImmediateMesh mesh)
    {

        foreach (KeyValuePair<Vector3i, long> item in AStarPositionToId[aStar])
        {
            Vector3i vector = item.Key;
            long point_id = item.Value;

            foreach (long connection_id in aStar.GetPointConnections(point_id))
            {
                Vector3i connection_vector = new Vector3i (aStar.GetPointPosition(connection_id));
                mesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
                mesh.DrawLine(vector.ToGVector3(), connection_vector.ToGVector3());
                mesh.SurfaceEnd();
            }

        }
    }


}
}
