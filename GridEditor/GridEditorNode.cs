using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.GenericGridStorage;

[GlobalClass, Tool]
public partial class GridEditorNode : Node3D
{
	private Grid Grid = null!;

	private List<Node3D> VisualNodes = new();

	public void UpdateGrid(Grid grid)
	{
		Grid = grid;

		VisualNodeClear();

		foreach (var cell in grid.CellGetAll())
		{
			VisualNodeAdd(cell);
		}
	}



	protected Cell RaycastFromCamera()
	{
		ThrowOnMissingGrid();

		Viewport viewport = GetViewport();
		Camera3D camera = viewport.GetCamera3D();
		Godot.Vector2 mousePos = viewport.GetMousePosition();

		Godot.Vector3 cameraPos = ToLocal(camera.GlobalPosition);
		Godot.Vector3 cameraDir = camera.ProjectRayNormal(mousePos);

		return Grid.IntersectRay(cameraPos, cameraDir);
	}

	protected void VisualNodeClear()
	{
		VisualNodes.ForEach(x => x.QueueFree());
		VisualNodes.Clear();
	}

	protected void VisualNodeAdd(Cell cell)
	{
		ThrowOnMissingGrid();

		Aabb aabb = Grid.GetAABB(cell.Position);
		MeshInstance3D meshInst = new()
		{
			Mesh = new BoxMesh() { Size = aabb.Size },
			Position = aabb.Position + (aabb.Size / 2),
		};

		VisualNodes.Add(meshInst);

		AddChild(meshInst);
	}

	private void ThrowOnMissingGrid()
	{
		if (Grid is null)
			throw new Exception("Must set a Grid before adding visuals.");
	}
}
