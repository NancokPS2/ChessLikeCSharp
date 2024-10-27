using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot;

public class BattleControllerStatePreparation : BattleControllerState
{
    private PartyMobListUI _unit_list;
    private Vector3i _last_selected_pos;
    private StandardMaterial3D _ghost_material = new(){
        Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.Always,
        AlbedoColor = new(1,1,1,0.5f)
        };
    public BattleControllerStatePreparation(BattleController.State identifier) : base(identifier)
    {

    }

    public override void StateOnEnter()
    {
        _unit_list = new PartyMobListUI().GetInstantiatedScene<PartyMobListUI>();
        User.CompCanvas.AddChild(_unit_list);
    }

    public override void StateOnExit()
    {
        _unit_list?.RemoveSelf();
    }

    public override void StateProcess(double delta)
    {
        Vector3i selected_pos = User.PositionSelected;
        Mob? selected_mob = _unit_list.MobSelected;
        GridNode.Layer layer = GridNode.Layer.MOB_GHOST;

        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.TAKING_TURN);
        }

        //Same position as last time.
        if (_last_selected_pos == selected_pos){return;}
        //No mob selected.
        if (selected_mob is null){return;}
        //Do nothing if the chosen space is occupied.
        if (Global.ManagerMob.GetInPosition(selected_pos).Count != 0){return;}

        //Set this position as the last valid one.
        _last_selected_pos = selected_pos;
        
        UpdateGhost(selected_pos, layer, selected_mob);

        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.ACCEPT))
        {
            MobPlace(selected_mob, selected_pos);
        }
        else if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.CANCEL))
        {
            List<Mob> mobs_to_remove = Global.ManagerMob.GetInPosition(selected_pos);
            foreach (var mob in mobs_to_remove)
            {
                MobRemove(mob);
            }
        }

    }

    public void UpdateGhost(Vector3i position, GridNode.Layer layer, Mob mob)
    {
        User.CompDisplayGrid.MeshRemove(layer);

        Mesh mob_mesh = mob.GetMeshInstance().Mesh;
        User.CompDisplayGrid.MeshSet(
            position, 
            layer, 
            mob_mesh
        );
        User.CompDisplayGrid.MeshGetInstance(position, layer)?.SetMaterialOverlay(_ghost_material);
    }

    public void MobPlace(Mob mob, Vector3i where)
    {
        if (Global.ManagerMob.GetInPosition(where).Count != 0)
        {
            MessageQueue.AddMessage("Cannot place unit, the space is occupied.", 2);
            return;
        }

        User.SetupParticipant(mob, where, true);
    }

    public void MobRemove(Mob mob)
    {
        User.RemoveParticipant(mob);
    }
}
