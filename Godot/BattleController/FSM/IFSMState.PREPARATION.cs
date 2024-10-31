using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot;

public class BattleControllerStatePreparation : BattleControllerState
{
    private PartyMobListUI _unit_list;
    private Vector3i _last_hovered_pos;
    private List<EFaction> FactionsEligible = new(){EFaction.PLAYER};
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
        _unit_list.Update(FactionsEligible);
        User.CompCanvas.AddChild(_unit_list);
        _unit_list.AnchorBottom = 0.4f;
    }

    public override void StateOnExit()
    {
        _unit_list?.RemoveSelf();
    }

    public override void StateProcess(double delta)
    {
        User.UpdateCursorMovement();
        User.UpdateCameraPosition(delta);
        User.UpdateHoveredMobUI();

        Mob? selected_mob = _unit_list.MobSelected;
        Vector3i selected_pos = User.PositionSelected;
        Vector3i hovered_pos = User.PositionHovered;

        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.TAKING_TURN);
        }

        //Set this position as the last valid one.
        _last_hovered_pos = hovered_pos;

        //Do nothing if the chosen space is occupied.
        if (Global.ManagerMob.GetInPosition(selected_pos).Count != 0){return;}

        if (Global.GInput.IsButtonPressed(Global.GInput.Button.ACCEPT))
        {
            //No mob selected, return.
            if (selected_mob is null){return;}
            //Invalid position, return.
            if (!selected_pos.IsValid()) {return;}

            MobPlace(selected_mob, selected_pos);
        }
        else if (Global.GInput.IsButtonPressed(Global.GInput.Button.CANCEL))
        {
            //Invalid position, return.
            if (!hovered_pos.IsValid()) {return;}

            //Get all mobs of eligible factions.
            List<Mob> mobs_to_remove = 
                Global.ManagerMob.GetInPosition(hovered_pos).Where(
                    x => FactionsEligible.Contains(x.Faction)
                ).ToList();

            foreach (var mob in mobs_to_remove)
            {
                MobRemove(mob);
            }
        }

    }

    //TODO: Unimplemented.
    public void UpdateGhost(Vector3i position, Mob mob)
    {
        bool same_position = _last_hovered_pos == position;
        bool pos_invalid = !position.IsValid();
        if (same_position || pos_invalid) {return;}

        GridNode.Layer layer = GridNode.Layer.MOB_GHOST;

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
