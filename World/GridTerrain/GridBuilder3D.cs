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
    public int HeightMax = 8;

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

    [ExportToolButton("Save")]
    public Callable SaveCall
    {
        get => Callable.From(Save);
    }

    public override void _Ready()
    {
        base._Ready();
        AddChild(GridNodeUsed);
    }

    public void Save()
    {
        
    }

    private void SetTerrainHeightMap(NoiseTexture2D map)
    {
        Godot.Image image = map.GetImage();
        GridUsed.Boundary = new(map.GetWidth(), HeightMax, map.GetHeight());
        List<Vector3i> chosen = new();
        for (int x = 0; x < GridUsed.Boundary.X; x++)
        {
            for (int y = 0; y < GridUsed.Boundary.Y; y++)
            {
                float heightValue = image.GetPixel(x, y).Luminance;
                for (int z = 0; z < GridUsed.Boundary.Z; z++)
                {
                    if (heightValue > ((float)z / (float)HeightMax)) chosen.Add(new(x, y, z));
                }
            }
        }
        foreach (var item in chosen)
        {
            GridUsed.SetCell(item, GridCell.Preset.Floor);
        }

        GridNodeUsed.SetGrid(GridUsed);
    }
}
