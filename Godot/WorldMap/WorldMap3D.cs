using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot.WorldMap;

public partial class WorldMap3D : Node, ISelectableCollection<MapMarker3D>
{
    private Dictionary<MapMarker3D, (Node3D, MeshInstance3D, Area3D, Label3D)> MarkerNodes = new();
    private List<MapMarker3D> markers = new();
    private int maxItemsSelected = 1;

    public List<MapMarker3D> Markers { get => markers; set => MarkersSet(value); }
    public int MaxItemsSelected { get => maxItemsSelected; set => maxItemsSelected = value; }

    public void MarkersSet(ICollection<MapMarker3D> collection)
    {
        Markers = collection.ToList();
        GetTree().ProcessFrame += UpdateNodes;
    }

    private void UpdateNodes()
    {
        //Clean nodes.
        GetTree().ProcessFrame -= UpdateNodes;
        foreach (var node in MarkerNodes.Keys)
        {
            node.QueueFree();
        }

        foreach (var item in Markers)
        {
            Node3D main_node = new(){Position = item.Position};
            AddChild(main_node);

            Label3D label = new(){Text = item.DisplayedName, Position = Vector3.Up};
            main_node.AddChild(label);

            //Input
            Area3D area = new(){InputRayPickable = true};
            CollisionShape3D collision = new(){Shape = new SphereShape3D(){Radius = 0.35f}};
            area.AddChild(collision);
            area.InputEvent += 
                (cam, eve, pos, nor, idx) 
                    => MarkerInput(cam, eve, pos, nor, idx, item);
            main_node.AddChild(area);

            MeshInstance3D mesh_node = new(){Mesh = item.VisualMesh};
            main_node.AddChild(mesh_node);

            //Update entry
            MarkerNodes[item] = (main_node, mesh_node, area, label);
        }
    }

    private void MarkerInput(Node camera, InputEvent input_event, Vector3 event_pos, Vector3 normal, long shape_idx, MapMarker3D marker)
    {
        if (input_event.IsActionPressed(Global.GInput.GetActionName(Global.GInput.Button.ACCEPT)))
        {
            EventBus.MarkerSelected?.Invoke(marker);
        } else if (input_event is InputEventMouseMotion motion)
        {
            marker.Selected = true;
        }

    }

    public void UpdateHighlights()
    {
        var list = GetSelectables();
        foreach (var item in list)
        {
            Label3D label = MarkerNodes[item].Item4;
            label.Modulate = item.Selected ? label.Modulate = Colors.Green : label.Modulate = Colors.White;
        }
    }

    public List<MapMarker3D> GetSelectables() => Markers;

    public List<MapMarker3D> GetSelected() => Markers.Where(x => x.Selected).ToList();
}
