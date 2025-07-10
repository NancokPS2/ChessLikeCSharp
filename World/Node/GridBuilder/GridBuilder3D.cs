using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
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

    [ExportCategory("Nodes and Scenes")]
    [Export]
    protected GridNode GridNodeUsed;

    [Export]
    protected GridMap GridMapNode;

    [Export]
    protected PackedScene MarkerScene;
    protected Node3D MarkerNode;
    protected Vector3i MarkerNodePosition;

    [ExportCategory("Main")]
    /// <summary>
    /// The actual max height of the terrain.
    /// </summary>
    [Export]
    public int HeightMax = 8;

    [ExportToolButton("Show GridNode")]
    public Callable ShowGridNodeCall
    {
        get => Callable.From(ShowInGridNode);
    }

    [ExportToolButton("Sanitize GridMap")]
    public Callable SanitizeGridMapCall
    {
        get => Callable.From(SanitizeGridMap);
    }

    [ExportCategory("Terrain Generation")]
    /// <summary>
    /// The height that the terrain can reach. If set to 1, the terrain will usually touch the top of the Grid.
    /// </summary>
    [Export(PropertyHint.Range, "0,1,0.01")]
    public float TerrainHeightModifier = 1;

    [Export]
    protected Texture2D? TerrainHeightMap;

    [ExportToolButton("Generate Terrain")]
    protected Callable TerrainGenerateCall
    {
        get => Callable.From(TerrainGenerationCall);
    }

    [ExportCategory("Loading")]
    [Export]
    protected Grid? GridToLoad;

    [ExportToolButton("Load")]
    protected Callable LoadCall
    {
        get => Callable.From(LoadingCall);
    }

    [ExportToolButton("Reload GridMap")]
    protected Callable ReloadGridMapCall
    {
        get => Callable.From(UpdateGridMapFromGrid);
    }

    [ExportCategory("Saving")]

    [Export]
    protected bool AutoFillEmptySpaceWithAir = true;

    [Export(PropertyHint.SaveFile, "*.tres")]
    public string SavePath = "user://SavedGrid.tres";

    [ExportToolButton("Save")]
    public Callable SaveCall
    {
        get => Callable.From(SavingCall);
    }

    public override void _Ready()
    {
        base._Ready();
        MarkerNode = MarkerScene.Instantiate<Node3D>();
        AddChild(MarkerNode);

        GridMapNode.MeshLibrary = null;
        GridMapNode.Clear();
    }

    #region Terrain Generation

    private void TerrainGenerationCall()
    {
        GridMapNode.Clear();

        if (TerrainHeightMap is not null)
        {
            if (TerrainHeightMap.GetWidth() > MAX_SIZE || TerrainHeightMap.GetHeight() > MAX_SIZE)
            {
                GD.PushError($"Image is larger than {MAX_SIZE}x{MAX_SIZE}.");
                return;
            }
            SetGridMapToHeightMap(TerrainHeightMap);
        }

        UpdateGridFromGridMap();
    }

    private void SetGridMapToHeightMap(Texture2D map)
    {
        Godot.Image image = map.GetImage();
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
    }
    #endregion

    #region Saving and Loading
    private void SavingCall()
    {
        UpdateGridFromGridMap();

        if (AutoFillEmptySpaceWithAir)
            GridUsed.FillCell(GridCell.Preset.Air, true);

        var error = ResourceSaver.Save(GridUsed, SavePath);
        if (error != Error.Ok) GD.PushError($"Failed to save Grid with error: {error}");
    }

    public void LoadingCall()
    {
        if (GridToLoad is null)
        {
            GD.PushError("Nothing to load.");
            return;
        }
        GridUsed = GridToLoad;
        UpdateGridMapFromGrid();
    }

    protected void SanitizeGridMap()
    {
        foreach (var item in GridUsed.CellDictionary.Keys)
        {
            if (item.X < 0 || item.Y < 0 || item.Z < 0)
                GridMapNode.SetCellItem(item, -1);
        }
    }
    #endregion

    protected void ShowInGridNode()
    {
        bool show = !GridNodeUsed.Visible;

        GridNodeUsed.Visible = show;
        GridMapNode.Visible = !show;
        if (show)
        {
            UpdateGridFromGridMap();
            GridNodeUsed.SetGrid(GridUsed);
        }
    }

    protected MeshLibrary GetMeshLibFromGridCells(List<GridCell>? gridCells)
    {
        gridCells ??= GridCell.Preset.GetAll();
            
        MeshLibrary output = new();

        int id = 0;
        foreach (var item in gridCells)
        {
            AddMeshToLibrary(output, id, item);
            id++;
        }
        return output;
    }

    private void AddMeshToLibrary(MeshLibrary meshLib, int id, GridCell item)
    {
        if (id > ColorList.Count)
            throw new Exception($"We only have {ColorList.Count} colors, but id is {id}.");
        if (id < meshLib.GetItemList().Count())
            throw new Exception("We are REPLACING an item!");

        Godot.Vector3 cellSize = Grid.CellSize;

        Godot.Color color = ColorList[id];
        color.A = item.Name == "Air" ? 0.5f : 0.75f;
        StandardMaterial3D material = new()
        {
            AlbedoColor = color,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha
        };
        BoxMesh mesh = new()
        {
            Material = material,
            Size = cellSize
        };
        BoxShape3D shape = new() { Size = cellSize };

        Godot.Image image = Godot.Image.CreateEmpty(
                2,
                2,
                false,
                Godot.Image.Format.Rgbah
                );
        image.Fill(color);
        ImageTexture texture = ImageTexture.CreateFromImage(image);

        meshLib.CreateItem(id);
        meshLib.SetItemName(id, item.Name);
        meshLib.SetItemMesh(
            id,
            mesh
            );
        meshLib.SetItemShapes(
            id,
            new() { shape }
            );
        meshLib.SetItemPreview(
            id,
            texture
        );
    }

    protected int GetGroundIDFromMeshLibrary(MeshLibrary meshLibrary)
    {
        foreach (var id in meshLibrary.GetItemList())
        {
            if (meshLibrary.GetItemName(id) == "Ground") return id;
        }
        throw new Exception("Ground not found in MeshLibrary.");
    }

    protected void UpdateGridFromGridMap()
    {
        foreach (var item in GridMapNode.GetUsedCells())
        {
            int id = GridMapNode.GetCellItem(item);
            string name = GridMapNode.MeshLibrary.GetItemName(id);
            GridUsed.SetCell(new(item), GridCell.Preset.GetByName(name));
        }
    }

    protected void UpdateGridMapFromGrid()
    {
        GridMapNode.Clear();
        GridMapNode.MeshLibrary = null;

        MeshLibrary newLibrary = new();
        GridMapNode.MeshLibrary = newLibrary;

        Dictionary<GridCell, int> uniqueCells = new();
        int id = 0;
        foreach (var item in GridUsed.CellDictionary)
        {
            //If we do not have an item in the MeshLibrary for this kind of GridCell, make a new one.
            if (!uniqueCells.ContainsKey(item.Value))
            {
                AddMeshToLibrary(newLibrary, id, item.Value);
                uniqueCells[item.Value] = id;
                id++;
            }

            int libItemId = uniqueCells[item.Value];
            GridMapNode.SetCellItem(item.Key, libItemId);
        }
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
