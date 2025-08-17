using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessLike.Entity;
using Godot;

namespace ChessLike.World;

public static class GridTerrainGenerator
{
	public static Grid GenerationPassFillBottomHoles(this Grid grid, GridCell floorCell)
	{
		for (int x = 0; x < grid.Boundary.X; x++)
		{
			for (int z = 0; z < grid.Boundary.Z; z++)
			{
				Vector3i pos = new(x, 0, z);
				//Continue if it is already solid.
				if (grid.HasCell(pos) && grid.IsFlagInPosition(pos, ECellFlag.SOLID)) continue;

				grid.SetCell(pos, floorCell);
			}
		}
		return grid;
	}

	public static Grid GenerationPassNoiseTerrain(this Grid grid, Noise noise, float heightModifier, GridCell cellToApply)
	{
		int maxHeight = (int)(grid.Boundary.Y * heightModifier);

		foreach (var item in Vector3i.Range(grid.Boundary.X, grid.Boundary.Y, grid.Boundary.Z))
		{
			float noiseVal = Mathf.Abs(noise.GetNoise2Dv(new(item.X, item.Z)));
			if (maxHeight * noiseVal >= item.Y)
				grid.SetCell(item, cellToApply);
		}

		return grid;
	}

	public static Grid GenerationpPassFillEmpty(this Grid grid, GridCell gridCellToApply)
	{
		foreach (var item in grid.GetInboundPositions())
		{
			if (grid.HasCell(item)) continue;

			grid.SetCell(item, gridCellToApply);
		}
		return grid;
	}

	public static Grid GenerationPassPlaceSpawnpointsOppositeEnds(this Grid grid, EFaction firstFac, EFaction secondFac)
	{
		GridCell spawnPointOne = new GridCell(grid.GetCell(grid.FindFirstNonSolidPositionVertical(Vector3i.ZERO)));
		GridCell spawnPointTwo = new(spawnPointOne);

		spawnPointOne.FactionSpawn = firstFac;
		spawnPointTwo.FactionSpawn = secondFac;

		for (int count = 0; count < grid.Boundary.Z; count++)
		{
			Vector3i posOne = grid.FindFirstNonSolidPositionVertical(new(0, 0, count));
			Vector3i posTwo = grid.FindFirstNonSolidPositionVertical(new(grid.Boundary.X - 1, 0, count));
			grid.SetCell(posOne, spawnPointOne);
			grid.SetCell(posTwo, spawnPointTwo);
		
		}	
		return grid;
	}

    public static Grid GenerateFlat(Vector3i size)
	{
		Grid output = new();
		output.Boundary = size;

		int[] X = Enumerable.Range(0, (int)size.X).ToArray();
		int[] Y = Enumerable.Range(0, (int)size.Y).ToArray();
		int[] Z = Enumerable.Range(0, (int)size.Z).ToArray();
		foreach (int ind_x in X)
		{
			foreach (int ind_y in Y)
			{
				foreach (int ind_z in Z)
				{
					Vector3i position = new(ind_x, ind_y, ind_z);
					if (position.Y < 1)
					{
						output.SetCell(position, GridCell.Preset.Floor);
					}
					else if (position.X == 0 && position.Y == 1)
					{
						output.SetCell(position, GridCell.Preset.Spawnpoint);
					}
					else
					{
						output.SetCell(position, GridCell.Preset.Air);
					}
				}
			}

		}
		Debug.Assert(output.CellDictionary.ContainsKey(size - Vector3i.ONE));

		return output;
	}
}

