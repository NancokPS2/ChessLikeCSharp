using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
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
        EventBus.InputPreparationFinished += OnInputPreparationFinished;
    }

    protected bool PlaceMob(Vector3i cellPos, GridCell cell, Mob selectedMob)
    {
        //Mob must be selected
        if (selectedMob is null) return false;

        //The position must be valid for this mob.
        if (!selectedMob.IsValidPositionToExist(CombatScene.GetGrid(), cellPos))
        {
            MessageQueue.AddMessage($"{selectedMob.DisplayedName} cannot stand there.");
            return false;
        }

        //Must be a valid spot to place mobs
        if (cell.FactionSpawn != selectedMob.Faction)
        {
            MessageQueue.AddMessage($"{selectedMob.DisplayedName} cannot start there.");
            return false;
        }

        //If it passed all checks, add it to combat.
        if (selectedMob.MobState != EMobState.COMBAT)
        {
            selectedMob.MobState = EMobState.COMBAT;
        }

        selectedMob.Move(cellPos);
        return true;
    }

    protected bool RemoveMob(Vector3i cellPos)
    {
        List<Mob> mobs = Global.ManagerMob.GetPooledInCombat().FilterInPosition(cellPos);

        if (mobs.IsEmpty())
        {
            return false;
        }
        else
        {
            mobs.First().MobState = EMobState.BENCHED;
            return true;
        }
    }

    #region Event Handling
    private void OnInputPreparationFinished()
    {
        EventBus.CombatPreparationEnded?.Invoke();
    }

    private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
    {
        if (CombatScene.GetState() != ECombatState.PREPARATION) return;
        bool placedMob;
        bool removedMob;

        //Must be a PRIMARY input
        switch (input)
        {
            case ECellInput.PRIMARY:
                if (SelectedMob is null) break;
                placedMob = PlaceMob(cellPos, cell, SelectedMob);
                break;

            case ECellInput.SECONDARY:
                removedMob = RemoveMob( cellPos);
                break;

            default: break;
        }

    }

    private void OnMobSelected(Mob obj)
    {
        if (CombatScene.GetState() != ECombatState.PREPARATION) return;
        SelectedMob = obj;
    }
    #endregion
}
