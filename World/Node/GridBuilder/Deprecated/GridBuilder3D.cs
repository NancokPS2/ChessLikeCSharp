using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

    [ExportCategory("Nodes and Scenes")]
    [Export]
    protected GridNode GridNodeUsed;

    [Export]
    protected GridMap GridMapNode;
    
    [ExportCategory("Main")]

    public Vector3i Boundary = new(16, 8, 16);
    [Export]
    private Vector3I boundary
    {
        set => Boundary = new(value);
        get => Boundary.ToGVector3I();
    }

    [Export]
    protected Godot.Collections.Array<GridCell> GridCellsToLoad = new();

    [ExportToolButton("Update MeshLibrary")]
    protected Callable UpdateMeshLibraryCall
    {
        get => Callable.From( UpdateMeshLibrary );
    }

    [ExportToolButton("Show GridNode")]
    public Callable ShowGridNodeCall
    {
        get => Callable.From(ShowGridNode);
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
        get => Callable.From(Load);
    }

    [ExportCategory("Saving")]

    [Export]
    protected bool AutoFillEmptySpaceWithAir = true;

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
    }

    private void SetGridMapToHeightMap(Texture2D map)
	{
		Godot.Image image = map.GetImage();
		
		for (int x = 0; x < Boundary.X; x++)
		{
			for (int z = 0; z < Boundary.Z; z++)
			{
				float heightValue = image.GetPixel(x, z).Luminance;
				heightValue *= TerrainHeightModifier;

				for (int y = 0; y < Boundary.Y; y++)
				{
					if (heightValue > ((float)y / (float)Boundary.Y))
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

	#region Calls
	private void Save()
    {
        SanitizeGridMap();
        Grid newGrid = GetNewGrid();


        foreach (var item in GridMapNode.GetUsedCells())
        {
            int id = GridMapNode.GetCellItem(item);
            GridCell gridCell = GridMapNode.MeshLibrary.GetItemMeta(id).As<GridCell>();
            newGrid.SetCell(new(item), gridCell);
        }

        if (AutoFillEmptySpaceWithAir)
            newGrid.FillCell(GridCell.Preset.Air, true);

        var error = ResourceSaver.Save(newGrid, SavePath);
        if (error != Error.Ok) GD.PushError($"Failed to save Grid with error: {error}");
    }

    public void Load()
    {
        if (GridToLoad is null)
        {
            GD.PushError("Nothing to load.");
            return;
        }
        UpdateGridMapFromGrid(GridToLoad);
    }

    protected void ShowGridNode()
    {
        bool show = !GridNodeUsed.Visible;

        GridNodeUsed.Visible = show;
        GridMapNode.Visible = !show;
        if (show)
        {
            GridNodeUsed.SetGrid(GetNewGrid());
        }
    }
    #endregion

    #region GridMap
    protected void SanitizeGridMap()
    {
        foreach (var item in GridMapNode.GetUsedCells())
        {
            if (!Grid.IsPositionInbounds(new(item), Boundary))
                GridMapNode.SetCellItem(item, (int)GridMap.InvalidCellItem);
        }
    }

    protected void UpdateMeshLibrary()
    {
        if (GridMapNode is null) return;
        GridMapNode.MeshLibrary = new();
        GridMapNode.MeshLibrary = GetMeshLibFromGridCells(
            new(
                from gridCell
                in GridCellsToLoad
                select gridCell is not null ? gridCell : GridCell.Preset.Air)
            );
    }

    protected void UpdateGridMapFromGrid(Grid grid)
    {
        GridMapNode.Clear();
        GridMapNode.MeshLibrary = null;

        Boundary = grid.Boundary;
        GridCellsToLoad.Clear();

        Dictionary<GridCell, int> gridCellToId = new();
        Dictionary<Vector3i, int> placementDict = new();

        int id = 0;
        foreach (var item in grid.CellDictionary)
        {
            if (!GridCellsToLoad.Contains(item.Value))
            {
                GridCellsToLoad.Add(item.Value);
                gridCellToId[item.Value] = id;
                id++;
            }
            placementDict[item.Key] = gridCellToId[item.Value];
        }
        UpdateMeshLibrary();
        foreach (var item in placementDict)
        {
            GridMapNode.SetCellItem(item.Key, item.Value);
        }
    }
    #endregion

    #region MeshLibrary
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
        color.A = item.Flags.Contains(ECellFlag.AIR) ? 0.05f : 0.75f;
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
        meshLib.SetItemMeshCastShadow(
            id,
            RenderingServer.ShadowCastingSetting.Off
            );
        if (meshLib.GetItemMeta(id).As<GridCell>() != item)
            throw new Exception($"{id} does not have the metadata that was just set.");
    }
    #endregion

    #region Grid
    protected Grid GetNewGrid()
    {
        Grid grid = new() { Boundary = Boundary };
        foreach (var vector in GridMapNode.GetUsedCells())
        {
            int id = GridMapNode.GetCellItem(vector);
            GridCell gridCell = GridMapNode.MeshLibrary.GetItemMeta(id).As<GridCell>();
            grid.SetCell(new(vector), gridCell);
        }
        return grid;
    }
    #endregion
}
