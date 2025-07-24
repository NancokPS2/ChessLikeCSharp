using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using Godot;

[GlobalClass]
public partial class CombatPreparationManager : Node3D
{
    protected Mob? SelectedMob;

    public override void _Ready()
    {
        base._Ready();
        EventBus.MobSelected += OnMobSelected;
        EventBus.CellInputReceived += OnCellInputReceived;
    }

    private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
    {
        if (CombatScene.GetState() != ECombatState.PREPARATION) return;

        //Must be a PRIMARY input
        if (input != ECellInput.PRIMARY) return;

        //Mob must be selected
        if (SelectedMob is null) return;

        //The position must be valid for this mob.
        if (!SelectedMob.IsValidPositionToExist(CombatScene.GetGrid(), cellPos)) return;
        
        SelectedMob.Move(cellPos);
    }

    private void OnMobSelected(Mob obj)
    {
        if (CombatScene.GetState() != ECombatState.PREPARATION) return;
        SelectedMob = obj;
    }
}
