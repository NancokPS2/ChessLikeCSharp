using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEventTargeter : Node3D
{
	protected Grid CurrentGrid;

	protected UsageParameters? UsageParametersCurrent;

	bool DisplayTargetingRange;

	public ActionEventTargeter()
	{
		EventBus.BattleStateChanged += OnBattleStateChanged;
		EventBus.EncounterLoading += OnEncounterLoading;
		EventBus.TargetingUsageParametersGenerated += OnTargetingUsageParametersGenerated;
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void TargetingRangeDisplay(ActionEvent action)
	{
	}



	#region Event Connection
	private void OnBattleStateChanged(EBattleState state)
	{
		DisplayTargetingRange = state == EBattleState.TARGETING;
		if (state == EBattleState.TARGETING)
		{

		}
		else UsageParametersCurrent = null;
	}

	private void OnTargetingUsageParametersGenerated(UsageParameters parameters)
	{
		UsageParametersCurrent = parameters;
	}

	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		if (UsageParametersCurrent is null) return;
		ActionEvent action = UsageParametersCurrent.ActionRef;

		switch (input)
		{
			case ECellInput.PRIMARY:
				break;

			case ECellInput.SECONDARY:
				break;

			default: throw new Exception();
		}
	}
	
	private void OnEncounterLoading(EncounterData obj)
	{
	}
	#endregion
}
