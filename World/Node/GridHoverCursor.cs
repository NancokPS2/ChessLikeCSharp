using ChessLike.World;
using Godot;
using System;

[GlobalClass]
public partial class GridHoverCursor : Node3D
{
	public override void _Ready()
	{
		base._Ready();
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		EventBus.CellInputReceived -= OnCellInputReceived;
	}


	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		if (input != ECellInput.HOVER) return;

		var pos = CombatScene.GetGridNode().MapToGlobal(cellPos);
		GlobalPosition = pos;
	}
}
