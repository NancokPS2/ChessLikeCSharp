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
    protected Grid GridUsed = new();
    protected GridNode GridNodeUsed = new();

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
        AddChild(GridNodeUsed);

        MarkerNode = MarkerScene.Instantiate<Node3D>();
        AddChild(MarkerNode);
    }

    public void Save()
    {
        var error = ResourceSaver.Save(GridUsed, SavePath);
        if (error != Error.Ok) GD.PushError($"Failed to save Grid with error: {error}");
    }

    private void SetTerrainHeightMap(NoiseTexture2D map)
    {
        Godot.Image image = map.GetImage();
        GridUsed.Boundary = new(map.GetWidth(), HeightMax, map.GetHeight());
        List<Vector3i> chosen = new();
        for (int x = 0; x < GridUsed.Boundary.X; x++)
        {
            for (int z = 0; z < GridUsed.Boundary.Z; z++)
            {
                float heightValue = image.GetPixel(x, z).Luminance;
                heightValue *= TerrainHeightModifier;

                for (int y = 0; y < GridUsed.Boundary.Y; y++)
                {
                    if (heightValue > ((float)y / (float)HeightMax)) chosen.Add(new(x, y, z));
                }
            }
        }
        foreach (var item in chosen)
        {
            GridUsed.SetCell(item, GridCell.Preset.Floor);
        }

        GridNodeUsed.SetGrid(GridUsed);
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
