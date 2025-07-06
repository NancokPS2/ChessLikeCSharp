using ChessLike.World;
using Godot;
using System;

[GlobalClass]
public partial class GridHoverCursor : Node3D
{
	public GridHoverCursor()
	{
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		if (input != ECellInput.HOVER) return;

		var pos = CombatScene.GetGridNode().MapToGlobal(cellPos);
		GlobalPosition = pos;
	}
}
