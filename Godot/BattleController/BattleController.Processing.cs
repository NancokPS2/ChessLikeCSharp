using ChessLike.Entity;

using ChessLike.Turn;
using ChessLike.World;
using static ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{

    const float MINIMUM_MOVEMENT_DELTA = 0.12f;
    private float _last_movement_time;
    public void UpdateCursorMovement()
    {
        //Decide between mouse based input and key input.
        if (CompDisplayGrid.InputEnabled)
        {
            if (!CompGrid.IsPositionInbounds(CompDisplayGrid.PositionHovered)){return;}

            PositionHovered = CompDisplayGrid.PositionHovered;
            CompDisplayGrid.MeshRemove(GridNode.Layer.CURSOR);
            CompDisplayGrid.MeshSet(
                PositionHovered, 
                GridNode.Layer.CURSOR, 
                Global.Resources.GetMesh(Global.Resources.MeshIdent.CURSOR)
                );
        }
        else
        {
            //delta must be high enough to continue
            if (Time.GetTicksMsec() - _last_movement_time < MINIMUM_MOVEMENT_DELTA){return;}

            _last_movement_time = Time.GetTicksMsec();
            Vector3i move = new Vector3i(Global.GInput.GetMovementVector(true));

            //Stop if there was no movement.
            if (move == Vector3i.ZERO){return;}

            //Ensure that it is valid before attempting the move.
            if ( CompGrid.IsPositionInbounds( move + PositionHovered ))
            {
                CompDisplayGrid.MeshRemove(GridNode.Layer.CURSOR);
                PositionHovered += move;
                CompDisplayGrid.MeshSet(PositionHovered, GridNode.Layer.CURSOR, Global.Resources.GetMesh(Global.Resources.MeshIdent.CURSOR));
            }else
            {
                GD.PushError(string.Format("Position {0} out of bounds.", (move + PositionHovered).ToString()));
            }
        }

    }

    public void UpdateCameraPosition(double delta)
    {
        Vector3 camera_pivot = CompCamera.pivot_point;

        if (camera_pivot.DistanceTo(PositionHovered.ToGVector3()) > 3)
        {
            camera_pivot = camera_pivot.Lerp(PositionHovered.ToGVector3(), (float)delta);
        }
    }

    public void UpdateMobUI()
    {
        //TODO
        List<Mob>? mob_list = Global.ManagerMob.GetInPosition(PositionHovered);
        if (mob_list.Count == 0) {return;}
        
        Mob mob = mob_list.First();

        if (mob is not null && CompMobCombatUI.CompUnitStatus.GetOwnerOfStats() != mob)
        {
            CompMobCombatUI.CompUnitStatus.UpdateStatNodes(mob);
        }
        //display_mob.MobUINode.UpdateStatNodes();
    }
}
