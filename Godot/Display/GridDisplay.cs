using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace Godot;

public partial class GridDisplay : Node3D
{
    public enum Layer
    {
        BASE,
        TARGETING,
        AOE,
        DECORATION,
        CURSOR,
        DEBUG,
    }
    private readonly Layer[] ALL_LAYERS = Enum.GetValues<Layer>();

    Grid grid;
    Dictionary<Vector3i, CellComponent> cell_components = new();

    public void SetGrid(Grid grid)
    {   
        this.grid = grid;
        
        //Clean existing nodes.
        foreach (Node node in GetChildren())
        {
            RemoveChild(node);
        }

        //Refill the list of cell components.
        cell_components.Clear();
        foreach (Vector3i position in grid.CellDictionary.Keys)
        {
            Grid.Cell cell = grid.CellDictionary[position];
            cell_components.Add(position, new CellComponent(cell));
            MeshRefresh(position);
        }
    }

    public void MeshSet(Vector3i position, Layer layer, Mesh? new_mesh)
    {
        if (new_mesh == null && cell_components.ContainsKey(position) && cell_components[position].mesh_instances.ContainsKey(layer))
        {
            cell_components[position].mesh_instances[layer].QueueFree();
            cell_components[position].mesh_instances.Remove(layer);
        }else
        {
            MeshInstance3D instance = new(){Mesh = new_mesh};
            cell_components[position].mesh_instances[layer] = instance;
            MeshRefresh(position);    
        }
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
        foreach (Vector3i position in cell_components.Keys)
        {
            MeshRemove(position, layer);
        }
    }

    public void MeshClearExtra(Vector3i position)
    {
        foreach (Layer layer in ALL_LAYERS)
        {
            if(layer == Layer.BASE){continue;}

            MeshRemove(position, layer);
        }
    }


    public void MeshRefresh(Vector3i position)
    {
        CellComponent component = cell_components[position];

        foreach (MeshInstance3D instance in component.mesh_instances.Values)
        {
            if (IsInstanceValid(instance) && !instance.IsInsideTree())
            {
                AddChild(instance);
            }
            instance.Position = position.ToGVector3();
        }
    }

}
