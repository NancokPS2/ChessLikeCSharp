using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Export]
    protected Godot.Collections.Array<GridCell> GridCellsLoaded
    {
        set
        {
            if (GridMapNode is null) return;
            GridMapNode.MeshLibrary = new();
            GridMapNode.MeshLibrary = GetMeshLibFromGridCells(
                new(
                    from gridCell
                    in value
                    select gridCell is not null ? gridCell : GridCell.Preset.Air)
                );
        }

        get
        {
            if (GridMapNode is null || GridMapNode.MeshLibrary is null) return new();
            return new(from id in GridMapNode.MeshLibrary.GetItemList() select GridMapNode.MeshLibrary.GetItemMeta(id).As<GridCell>());
        }
    }

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
    public int TerrainGroundMeshLibItem;

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
                            TerrainGroundMeshLibItem
                            );
                    }
                }
            }
        }
    }
    #endregion

    #region Saving and Loading
    private void SavingCall()
    {
        SanitizeGridMap();
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
            if (!GridUsed.IsPositionInbounds(item))
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
            AddGridCellToMeshLib(output, id, item);
            id++;
        }
        return output;
    }

    private void AddGridCellToMeshLib(MeshLibrary meshLib, int id, GridCell item)
    {
        if (id > ColorList.Count)
            throw new Exception($"We only have {ColorList.Count} colors, but id is {id}.");
        if (id < meshLib.GetItemList().Count())
            throw new Exception("We are REPLACING an item!");
        if (item is null)
            throw new Exception("Can't add a null item to the MeshLibrary");

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
        meshLib.SetItemMeta(
            id,
            item
            );
        if (meshLib.GetItemMeta(id).As<GridCell>() != item)
            throw new Exception($"{id} does not have the metadata that was just set.");
    }

    #region Updates
    protected void UpdateGridFromGridMap()
    {
        foreach (var vector in GridMapNode.GetUsedCells())
        {
            int id = GridMapNode.GetCellItem(vector);
            GridCell gridCell = GridMapNode.MeshLibrary.GetItemMeta(id).As<GridCell>();
            GridUsed.SetCell(new(vector), gridCell);
        }
        GridNodeUsed.SetGrid(GridUsed);
    }

    protected void UpdateGridMapFromGrid()
    {
        GridMapNode.Clear();
        GridMapNode.MeshLibrary = null;

        MeshLibrary newLibrary = new();
        GridMapNode.MeshLibrary = newLibrary;

        int id = 0;
        foreach (var item in GridUsed.CellDictionary)
        {
            //If we do not have an item in the MeshLibrary for this kind of GridCell, make a new one.
            if (newLibrary.FindIdWithMeta(item.Value) == -1)
            {
                AddGridCellToMeshLib(newLibrary, id, item.Value);
                if (newLibrary.FindIdWithMeta(item.Value) != id)
                    throw new Exception($"Could not find metadata for id {id}");
                id++;
            }

            int libItemId = newLibrary.FindIdWithMeta(item.Value);
            if (libItemId == -1) throw new Exception("Could not find any GridCell in the MeshLibrary.");
            GridMapNode.SetCellItem(item.Key, libItemId);
        }
    }
    #endregion
}
