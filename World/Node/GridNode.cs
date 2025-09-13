using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ChessLike.WorldMap;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace Godot;

[GlobalClass, Tool]
public partial class GridNode : Node3D, ISingleton<GridNode>
{
	public static GridNode Instance { get; set; } = null!;
    public enum Layer
    {
        BASE,
        TARGETING,
        AOE,
        DECORATION,
        CURSOR,
        DEBUG,
        MOB_GHOST,
        SPAWN_POINT,
    }
    private readonly Layer[] ALL_LAYERS = Enum.GetValues<Layer>();

	[Export]
	private Grid? EditorGrid
	{
		set
		{
			if (value is not null && Engine.IsEditorHint())
				SetGrid(value);
		}
		get
		{
			return grid;
		}
	}

	[ExportToolButton("Reload")]
	private Callable ReloadCall
	{
		get => Callable.From(() => EditorGrid = EditorGrid);
	}

	private Grid grid;
    private Dictionary<Vector3i, CellComponent> CellComponents = new();

    //public Vector3i PositionSelected { get => PositionCollidedSelected + Vector3i.UP; }
    //private Vector3i PositionCollidedSelected;

    //public Vector3i PositionHovered { get => PositionCollidedHovered + Vector3i.UP; }
    //private Vector3i PositionCollidedHovered;

    public Vector3 CellSize = new(1, 1, 1);

    public bool InputEnabled = true;

    private List<Vector3i> PosDirty = new();

	public override void _Ready()
	{
		base._Ready();
		EventBus.CombatGridChanged += OnGridChanged;
		Instance = this;
	}

	protected override void Dispose(bool disposing)
	{
		EventBus.CombatGridChanged -= OnGridChanged;
		base.Dispose(disposing);
	}


    #region Base

	public override void _Process(double delta)
	{
		base._Process(delta);
		foreach (var item in PosDirty)
		{
			MeshRefresh(item);
		}
		PosDirty.Clear();
	}

    public Grid GetGrid() => grid;

    public void SetGrid(Grid grid)
    {
        this.grid = grid;

        //Clean existing nodes.
        foreach (Node node in GetChildren())
        {
            RemoveChild(node);
        }

        //Refill the list of cell components.
        CellComponents.Clear();
        foreach (Vector3i position in grid.CellDictionary.Keys)
        {
            GridCell cell = grid.CellDictionary[position];
            CellComponent component = new CellComponent(cell);

            CellComponents.Add(position, component);

            MeshRefresh(position);

            CollisionConnect(position, component.CollisionBody);
            CollisionEnable(position, cell.Flags.Contains(ECellFlag.SOLID));

            //Custom stuff.
            if (grid.IsFlagInPosition(position, ECellFlag.SOLID))
            {
                MeshSet(position, Layer.BASE, Global.Resources.GetMesh(Global.Resources.MeshIdent.CELL_FULL));
            }
            if (grid.GetFactionSpawn(position) != ChessLike.Entity.EFaction.INVALID)
            {
                MeshSet(position, Layer.SPAWN_POINT, Global.Resources.GetMesh(Global.Resources.MeshIdent.SPAWNPOINT));
            }
        }
    }

    public Vector3 MapToLocal(Vector3i mapCoordinate, bool centered = false)
    {
        Vector3 pos = mapCoordinate.ToGVector3() * CellSize;
        if (centered) pos += CellSize / 2;
        return pos;
    }

    public Vector3 MapToGlobal(Vector3i mapCoordinate, bool centered = false)
        => ToGlobal(MapToLocal(mapCoordinate, centered));

    public Vector3i LocalToMap(Vector3 localCoordinate)
    {
        return new(localCoordinate / CellSize);
    }
    #endregion


    #region Collision
    protected void CollisionEnable(Vector3i position, bool enable)
    {
        CellComponent component = CellComponents[position];
        StaticBody3D body = component.CollisionBody;

        body.Position = position.ToGVector3();
        if (enable)
        {
            if (IsInstanceValid(body) && !body.IsInsideTree())
            {
                AddChild(body);
                body.AddChild(component.CollisionShape);
            }
            else
            {
                throw new Exception("The collision is not valid?");
            }
        }
        else if (body.GetParent() == this)
        {
            RemoveChild(body);
        }
    }

    protected void CollisionConnect(Vector3i position, StaticBody3D body)
    {
        body.InputEvent += (
            cam,
            input,
            pos,
            norm,
            shape
            ) => OnCellInput(input, position);
    }

    #endregion

    #region Mesh
    public void ModelSet(Vector3i position, Layer layer, Node3D? newModel)
    {
        if (newModel == null && CellComponents.ContainsKey(position) && CellComponents[position].MeshInstances.ContainsKey(layer))
        {
            MeshGetInstance(position, layer)?.QueueFree();
        }
    }
    public void MeshSet(Vector3i position, Layer layer, Mesh? new_mesh)
    {
        if (new_mesh == null && CellComponents.ContainsKey(position) && CellComponents[position].MeshInstances.ContainsKey(layer))
        {
            MeshGetInstance(position, layer)?.QueueFree();
            CellComponents[position].MeshInstances.Remove(layer);
        }
        else
        {
            MeshInstance3D instance = new() { Mesh = new_mesh };
            CellComponents[position].MeshInstances[layer] = instance;
        }
        PosDirty.Add(position);
    }

    public void MeshSet(List<Vector3i> positions, Layer layer, Mesh? new_mesh)
    {
        foreach (var item in positions)
        {
            MeshSet(item, layer, new_mesh);
        }
    }

    public MeshInstance3D? MeshGetInstance(Vector3i position, Layer layer)
    {
        if (CellComponents[position] is null) return null;
        else if (CellComponents[position].MeshInstances[layer] is null) return null;
        else return CellComponents[position].MeshInstances[layer];
    }

    public void MeshRemove(Vector3i position, Layer layer)
    {
        MeshSet(position, layer, null);
    }

    public void MeshRemove(Vector3i position, Layer[] layers)
    {
        if (layers.Count() == 0)
        {
            layers = ALL_LAYERS;
        }

        foreach (Layer layer in layers)
        {
            MeshRemove(position, layer);
        }
    }

    public void MeshRemove(Layer layer)
    {
        foreach (Vector3i position in CellComponents.Keys)
        {
            MeshRemove(position, layer);
        }
    }

    public void MeshClearNonBase(Vector3i position)
    {
        foreach (Layer layer in ALL_LAYERS)
        {
            if (layer == Layer.BASE) { continue; }

            MeshRemove(position, layer);
        }
    }


    public void MeshRefresh(Vector3i position)
    {
        CellComponent component = CellComponents[position];

        foreach (MeshInstance3D instance in component.MeshInstances.Values)
        {
            if (IsInstanceValid(instance) && !instance.IsInsideTree())
            {
                AddChild(instance);
            }
            instance.Position = position.ToGVector3();
        }
    }
    #endregion

    #region Event Handling
    public void OnCellInput(InputEvent input, Vector3i componentPos)
    {
        if (!InputEnabled) { return; }

        ECellInput cellInput = ECellInput.NONE;
        if (input.IsActionPressed("accept"))
        {
            cellInput = ECellInput.PRIMARY;
            EventBus.CellPositionSelected?.Invoke(componentPos + Vector3i.UP);
        }
        else if (input.IsActionPressed("cancel"))
        {
            cellInput = ECellInput.SECONDARY;
        }
        else if (input is InputEventMouseMotion)
        {
            cellInput = ECellInput.HOVER;
            EventBus.CellPositionHovered?.Invoke(componentPos + Vector3i.UP);
        }
        EventBus.CellPositionInputReceived?.Invoke(
            componentPos + Vector3i.UP,
            grid.GetCell(componentPos + Vector3i.UP),
            cellInput
            );
        EventBus.CellInputReceived?.Invoke(
            componentPos + Vector3i.UP,
            grid.GetCell(componentPos + Vector3i.UP),
            cellInput
            );
    }
	
	private void OnGridChanged(Grid obj)
	{
		SetGrid(obj);
	}

    #endregion

	#region Cell Component
	protected class CellComponent
	{
		public Dictionary<Layer, MeshInstance3D> MeshInstances = new();
		public StaticBody3D CollisionBody = new() { InputRayPickable = true };
		public CollisionShape3D CollisionShape = new() { Shape = new BoxShape3D() };

		public CellComponent(GridCell cell)
		{
			MeshInstances.Add(Layer.BASE, new MeshInstance3D());

			if (cell.Flags.Contains(ECellFlag.SOLID))
			{
				MeshInstances[Layer.BASE].Mesh = Global.Resources.GetMesh(Global.Resources.MeshIdent.CELL_FULL);
			}
		}

	}

    #endregion
}
