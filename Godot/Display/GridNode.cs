using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;

namespace Godot;

public partial class GridNode : Node3D
{

    public enum Layer
    {
        BASE,
        TARGETING,
        AOE,
        DECORATION,
        CURSOR,
        DEBUG,
        MOB_GHOST,
    }
    private readonly Layer[] ALL_LAYERS = Enum.GetValues<Layer>();

    private Grid grid;
    private Dictionary<Vector3i, CellComponent> CellComponents = new();
    private Node2D DrawNode = new();
    //private MeshInstance3D SelectedCursorInstance = new() { Mesh = new PrismMesh(){Size = new(1,-1,1)}};

    public Vector3i PositionCollidedSelected;
    public Vector3i PositionSelected {get => PositionCollidedSelected + Vector3i.UP;}
    public Vector3i PositionCollidedHovered;
    public Vector3i PositionHovered {get => PositionCollidedHovered + Vector3i.UP;}

    public bool InputEnabled = true;

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
            Grid.Cell cell = grid.CellDictionary[position];
            CellComponent component = new CellComponent(cell);

            CellComponents.Add(position, component);
            
            MeshRefresh(position);

            CollisionConnect(position, component.collision_body);
            CollisionEnable(position, cell.flags.Contains(CellFlag.SOLID));
        }
    }

    public void CollisionEnable(Vector3i position, bool enable)
    {
        CellComponent component = CellComponents[position];
        StaticBody3D body = component.collision_body;

        body.Position = position.ToGVector3();
        if (enable)
        {
            if (IsInstanceValid(body) && !body.IsInsideTree())
            {
                AddChild(body);
                body.AddChild(component.collision_shape);
            }
            else
            {
                throw new Exception("The collision is not valid?");
            }
        }
        else if(body.GetParent() == this)
        {
            RemoveChild(body);
        }
    }

    public void CollisionConnect(Vector3i position, StaticBody3D body)
    {
        body.InputEvent += (
            cam,
            input,
            pos,
            norm,
            shape 
            ) => OnCellInput(input, position);
    }

    public void MeshSet(Vector3i position, Layer layer, Mesh? new_mesh)
    {
        if (new_mesh == null && CellComponents.ContainsKey(position) && CellComponents[position].mesh_instances.ContainsKey(layer))
        {
            MeshGetInstance(position, layer)?.QueueFree();
            CellComponents[position].mesh_instances.Remove(layer);
        }else
        {
            MeshInstance3D instance = new(){Mesh = new_mesh};
            CellComponents[position].mesh_instances[layer] = instance;
            MeshRefresh(position);    
        }
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
        return CellComponents[position] is null || CellComponents[position].mesh_instances[layer] is null ? null 
        : CellComponents[position].mesh_instances[layer];
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
            if(layer == Layer.BASE){continue;}

            MeshRemove(position, layer);
        }
    }


    public void MeshRefresh(Vector3i position)
    {
        CellComponent component = CellComponents[position];

        foreach (MeshInstance3D instance in component.mesh_instances.Values)
        {
            if (IsInstanceValid(instance) && !instance.IsInsideTree())
            {
                AddChild(instance);
            }
            instance.Position = position.ToGVector3();
        }
    }

    public void OnCellInput(InputEvent input, Vector3i comp_position)
    {
        if (!InputEnabled){return;}

        if (input.IsPressed())
        {
            PositionCollidedSelected = comp_position;
            PositionCollidedHovered = comp_position;
        }
        else
        {
            PositionCollidedHovered = comp_position;
        }
    }
}
