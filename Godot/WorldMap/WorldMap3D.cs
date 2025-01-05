using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Godot.WorldMap;

public partial class WorldMap3D : Node, ISelectableList<MapMarker3D>
{
    private List<MapMarker3D> markers = new();
    private int maxItemsSelected = 1;
    private List<MapMarker3D> Markers { get => markers; set => MarkersSet(value); }

    public int MaxItemsSelected { get => maxItemsSelected; set => maxItemsSelected = value; }

    private void MarkersSet(ICollection<MapMarker3D> collection)
    {
        Markers = collection.ToList();
        GetTree().ProcessFrame += UpdateNodes;
    }

    private void UpdateNodes()
    {
        //Clean nodes.
        GetTree().ProcessFrame -= UpdateNodes;

        foreach (var marker in Markers)
        {
            if (!marker.IsInsideTree())
            {
                AddChild(marker);
            }
            

            //Input
            Area3D area = marker.NodeArea;;
            area.InputEvent += 
                (cam, eve, pos, nor, idx) 
                    => OnMarkerInput(cam, eve, pos, nor, idx, marker);
            area.MouseEntered += () => OnMarkerHover(marker);
        }
    }

    private void OnMarkerInput(Node camera, InputEvent input_event, Vector3 event_pos, Vector3 normal, long shape_idx, MapMarker3D marker)
    {
        if (input_event.IsActionPressed(Global.GInput.GetActionName(Global.GInput.Button.ACCEPT)))
        {
            EventBus.MarkerSelected?.Invoke(marker);
        } else if (input_event is InputEventMouseMotion motion)
        {
            marker.Selected = true;
        }
    }

    private void OnMarkerHover(MapMarker3D marker)
    {
        GetHovered().ForEach(x => x.Hovered = false);
        marker.Hovered = true;
    }

    public List<MapMarker3D> GetSelected() 
        => Markers.Where(x => x.Selected).ToList();

    public List<MapMarker3D> GetHovered() 
        => Markers.Where( x => x.Hovered ).ToList();

    public List<MapMarker3D> GetElements() => Markers;

    public void AddElement(MapMarker3D element)
    {
        Markers.Add(element);
        AddChild(element);
    }

    public void RemoveElement(MapMarker3D element)
    {
        Markers.Remove(element);
        element.QueueFree();
    }
}
