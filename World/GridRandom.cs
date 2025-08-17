using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.World;

[Tool, GlobalClass]
public partial class GridRandom : Grid
{
	public enum ESpawnPointPlacement
	{
		OPPOSITE,
	}

	[Export]
	protected ESpawnPointPlacement Type;

	[Export]
	protected GridCell EmptySpaceGridCell;

	[ExportCategory("Terrain")]
	[Export]
	protected Noise TerrainNoise;

	[Export]
	protected bool TerrainFillHoles = true;

	[Export(PropertyHint.Range, "0,1,0.1")]
	protected float TerrainHeightRatio = 0.4f;

	[Export]
	protected Godot.Collections.Array<GridCell> TerrainGridCells = new();


	[ExportCategory("Misc")]
	[ExportToolButton("Randomize")]
	protected Callable RandomizeCall
	{
		get => Callable.From(EditorRandomize);
	}

	public GridRandom()
	{
		//Callable.From(Randomize).CallDeferred();
		//Test this too:
		//Changed += EditorRandomize;
	}

	public void EditorRandomize()
	{
		if (Engine.IsEditorHint()) Randomize();
	}

	public void Randomize()
	{
		ClearCells();

		//Create terrain
		if (TerrainNoise is not null)
			this.GenerationPassNoiseTerrain(
				TerrainNoise,
				TerrainHeightRatio,
				TerrainGridCells.PickRandom());

		//Prevent holes into the void if enabled.
		if (TerrainFillHoles)
			this.GenerationPassFillBottomHoles(TerrainGridCells.PickRandom());

		//Fill empty space.
			this.GenerationpPassFillEmpty(EmptySpaceGridCell);

		//Place spawn points
		this.GenerationPassPlaceSpawnpointsOppositeEnds(Entity.EFaction.PLAYER, Entity.EFaction.NEUTRAL);
	}
}

