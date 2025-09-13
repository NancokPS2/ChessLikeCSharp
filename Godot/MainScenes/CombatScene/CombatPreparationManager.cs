using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
		var movement = MobMovementManager.Instance;

		//Make sure it is valid.
		if (!movement.IsPositionValidToSpawn(cellPos, selectedMob))
		{
			MsgLog.AddMessage($"{selectedMob.DisplayedName} cannot start there.");
			return false;
		}

		movement.ForceMobPosition(selectedMob, cellPos);
		return true;
	}

    protected bool RemoveMob(Vector3i cellPos)
    {
        List<Mob> mobs = Mob.GetInstancesInState(EMobState.COMBAT).FilterInPosition(cellPos);

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

        //Must be a PRIMARY input
        switch (input)
        {
            case ECellInput.PRIMARY:
                if (SelectedMob is null) break;
                PlaceMob(cellPos, cell, SelectedMob);
                break;

            case ECellInput.SECONDARY:
                RemoveMob( cellPos);
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
