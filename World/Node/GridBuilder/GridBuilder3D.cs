using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.World;

[GlobalClass, Tool]
public partial class GridBuilder3D : Node3D
{
    const int MAX_SIZE = 32;
    protected readonly List<Godot.Color> ColorList = new()
    {
        Colors.Red, Colors.Blue, Colors.Green,
        Colors.Gray, Colors.Purple, Colors.Brown,
        Colors.Black, Colors.Pink, Colors.Yellow
    };
    protected Grid GridUsed = new();

    [Export]
    protected GridNode GridNodeUsed;

    [Export]
    protected GridMap GridMapNode;

    [Export]
    protected PackedScene MarkerScene;
    protected Node3D MarkerNode;
    protected Vector3i MarkerNodePosition;

    /// <summary>
    /// The actual max height of the terrain
    /// </summary>
    [Export]
    public int HeightMax = 8;

    [ExportCategory("Terrain Generation")]
    /// <summary>
    /// The height that the terrain can reach.
    /// </summary>
    [Export(PropertyHint.Range, "0,1,0.01")]
    public float TerrainHeightModifier = 1;

    [Export]
    protected NoiseTexture2D? TerrainHeightMap
    {
        set
        {
            if (value?.GetWidth() > MAX_SIZE || value?.GetHeight() > MAX_SIZE)
                GD.PushError($"Image is larger than {MAX_SIZE}x{MAX_SIZE}.");
            if (value is null) return;
            SetTerrainHeightMap(value);
        }
        get => null;
    }
    
    [ExportCategory("Saving")]
    [Export(PropertyHint.SaveFile, "*.tres")]
    public string SavePath = "user://SavedGrid.tres";

    [ExportToolButton("Save")]
    public Callable SaveCall
    {
        get => Callable.From(Save);
    }

    public override void _Ready()
    {
        base._Ready();
        MarkerNode = MarkerScene.Instantiate<Node3D>();
        AddChild(MarkerNode);

        GridMapNode.MeshLibrary = GetMeshLibFromGridCells(null);
    }

    public void Save()
    {
        var error = ResourceSaver.Save(GridUsed, SavePath);
        if (error != Error.Ok) GD.PushError($"Failed to save Grid with error: {error}");
    }

    protected MeshLibrary GetMeshLibFromGridCells(List<GridCell>? gridCells)
    {
        gridCells ??= GridCell.Preset.GetAll();
        if (gridCells.Count > ColorList.Count)
            throw new Exception($"There are {gridCells.Count} GridCells but we only have {ColorList.Count} colors.");

        MeshLibrary output = new();
        Godot.Vector3 cellSize = Grid.CellSize;

        int id = 0;
        foreach (var item in gridCells)
        {
            Godot.Color color = ColorList[id];
            StandardMaterial3D material = new() { AlbedoColor = color };
            BoxMesh mesh = new() { Material = material, Size = cellSize };
            BoxShape3D shape = new() { Size = cellSize };

            output.CreateItem(id);
            output.SetItemName(id, item.Name);
            output.SetItemMesh(
                id,
                mesh
                );
            output.SetItemShapes(
                id,
                new() { shape }
                );
            id++;
        }
        return output;
    }

    private void SetTerrainHeightMap(NoiseTexture2D map)
    {
        Godot.Image image = map.GetImage();
        GridMapNode.Clear();
        GridUsed.Boundary = new(map.GetWidth(), HeightMax, map.GetHeight());
        //List<Vector3i> chosen = new();
        for (int x = 0; x < GridUsed.Boundary.X; x++)
        {
            for (int z = 0; z < GridUsed.Boundary.Z; z++)
            {
                float heightValue = image.GetPixel(x, z).Luminance;
                heightValue *= TerrainHeightModifier;

                for (int y = 0; y < GridUsed.Boundary.Y; y++)
                {
                    if (heightValue > ((float)y / (float)HeightMax))
                    {
                        GridMapNode.SetCellItem(
                            new(x, y, z), 
                            GetGroundIDFromMeshLibrary(GridMapNode.MeshLibrary)
                            );
                        //chosen.Add(new(x, y, z));
                    }
                }
            }
        }
        UpdateGridFromGridMap();
    }

    protected void UpdateGridFromGridMap()
    {
        foreach (var item in GridMapNode.GetUsedCells())
        {
            int id = GridMapNode.GetCellItem(item);
            string name = GridMapNode.MeshLibrary.GetItemName(id);
            GridUsed.SetCell(new(item), GridCell.Preset.GetByName(name));
        }
        UpdateGridNode();
    }

    protected void UpdateGridMapFromGrid()
    {
        foreach (var item in GridUsed.CellDictionary)
        {
            
        }
    }

    protected void UpdateGridNode()
    {
        GridNodeUsed.SetGrid(GridUsed);
    }

    protected int GetGroundIDFromMeshLibrary(MeshLibrary meshLibrary)
    {
        foreach (var id in meshLibrary.GetItemList())
        {
            if (meshLibrary.GetItemName(id) == "Ground") return id;
        }
        throw new Exception("Ground not found in MeshLibrary.");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Vector3i newPos = MarkerNodePosition;
        if (Input.IsActionJustPressed("ui_up"))
        {
            newPos += Vector3i.FORWARD;
        }
        else if (Input.IsActionJustPressed("ui_down"))
        {
            newPos += Vector3i.BACK;
        }
        else if (Input.IsActionJustPressed("ui_left"))
        {
            newPos += Vector3i.LEFT;
        }
        else if (Input.IsActionJustPressed("ui_right"))
        {
            newPos += Vector3i.RIGHT;
        }

        if (!GridUsed.IsPositionInbounds(newPos)) return;

        MarkerNode.GlobalPosition = GridNodeUsed.MapToGlobal(newPos);
    }
}
