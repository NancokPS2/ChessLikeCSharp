using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;

namespace Godot;

public partial class GridDisplay : Node3D
{
    Grid grid;
    List<CellComponents> cell_components = new();


    public void LoadGrid(Grid grid)
    {   
        this.grid = grid;
        
        //Clean existing nodes.
        foreach (Node node in GetChildren())
        {
            RemoveChild(node);
        }

        //Refill the list of cell components.
        cell_components.Clear();
        foreach (Vector3i position in grid.cells_dictionary.Keys)
        {
            Cell cell = grid.cells_dictionary[position];
            cell_components.Add(new(cell, position));
        }

        //Add the nodes from the components.
        foreach (CellComponents component in cell_components)
        {
            AddChild(component.mesh_instance);
        }
    }
}
