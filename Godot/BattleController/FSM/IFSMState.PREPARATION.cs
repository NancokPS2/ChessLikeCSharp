using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot;

public class BattleControllerStatePreparation : BattleControllerState
{
    private PartyMobListUI? _unit_list;
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
        UI.GetLayer(UI.ELayer.BASE_LAYER).AddChild(_unit_list);
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

        Mob? selected_mob = _unit_list?.MobSelected ?? null;
        Vector3i hovered_pos = User.PositionHovered;

        //If attempted to PAUSE, exit PREPARATION.
        //TODO: Ask for confirmation.
        if (Global.GInput.IsButtonJustPressed(Global.GInput.Button.PAUSE))
        {
            User.FSMSetState(BattleController.State.TAKING_TURN);
        }

        //If accepted.
        if (Global.GInput.IsButtonPressed(Global.GInput.Button.ACCEPT))
        {
            //No mob selected, return.rr
            if (selected_mob is null){return;}

            //Must be a valid and unoccupied position.
            if (!IsValidSpawnPoint(hovered_pos) && !IsPointOccupied(hovered_pos))
            {
                return;
            }

            //A move is selected and a valid position was selected. Place the unit.
            MobPlace(selected_mob, hovered_pos);
        }

        //If cancelled, try to remove the mob from the chosen location.
        else if (Global.GInput.IsButtonPressed(Global.GInput.Button.CANCEL))
        {

            //Get all mobs of eligible factions at the given location.
            List<Mob> mobs_to_remove = 
                Global.ManagerMob.GetInPosition(hovered_pos).Where(
                    x => FactionsEligible.Contains(x.Faction)
                ).ToList();

            foreach (var mob in mobs_to_remove)
            {
                MobRemove(mob);
            }
        }

        //Set this position as the last valid one.
        if (IsValidSpawnPoint(hovered_pos) && !IsPointOccupied(hovered_pos))
        {
            _last_hovered_pos = hovered_pos;
        }
    }

    //TODO: Unimplemented.
    public void UpdateGhost(Vector3i position, Mob mob)
    {
        bool same_position = _last_hovered_pos == position;
        bool pos_invalid = !IsValidSpawnPoint(position);
        if (same_position || pos_invalid) {return;}

        GridNode.Layer layer = GridNode.Layer.MOB_GHOST;

        BattleController.CompDisplayGrid.MeshRemove(layer);

        Mesh mob_mesh = mob.GetMeshInstance().Mesh;
        BattleController.CompDisplayGrid.MeshSet(
            position, 
            layer, 
            mob_mesh
        );
        BattleController.CompDisplayGrid.MeshGetInstance(position, layer)?.SetMaterialOverlay(_ghost_material);
    }

    public bool IsValidSpawnPoint(Vector3i position)
    {
        bool is_valid_position = position.IsValid();
        bool is_spawn_point = BattleController.CompGrid.IsFlagInPosition(
            position, ChessLike.World.ECellFlag.PLAYER_SPAWNPOINT
            );

        return is_valid_position && is_spawn_point;
    }

    public bool IsPointOccupied(Vector3i position) => Global.ManagerMob.GetInPosition(position).Count != 0;

    public void MobPlace(Mob mob, Vector3i where)
    {
        //Place mobs only if the location is empty.
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
