using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public class BattleControllerStateTargeting : BattleControllerState
{
    public BattleControllerStateTargeting(BattleController.State identifier) : base(identifier)
    {
    }

    public override void StateOnEnter()
    {
        List<Vector3i> range_to_mark = User.InputActionSelected.TargetingGetPositionsInRange(User.TurnUsageParameters);

        GD.Print(range_to_mark.ToArray());

        User.CompDisplayGrid.MeshSet(
            range_to_mark, 
            GridNode.Layer.TARGETING, 
            Global.Resources.GetMesh(Global.Resources.MeshIdent.TARGETING_TARGETABLE)
            );
    }

    public override void StateOnExit()
    {
        User.CompDisplayGrid.MeshRemove(GridNode.Layer.TARGETING);
        User.CompDisplayGrid.MeshRemove(GridNode.Layer.AOE);
    }

    public override void StateProcess(double delta)
    {
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.PAUSED);
        }
        User.UpdateCursorMovement();

        //If cancelled, return to AWAITING_ACTION.
        User.UpdateCameraPosition(delta);
        User.ProcessTargetingState(delta);
        User.UpdateMobUI();
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.CANCEL))
        {
            User.InputActionSelected = null;
            User.FSMSetState(BattleController.State.AWAITING_ACTION);
        }
    }
}
