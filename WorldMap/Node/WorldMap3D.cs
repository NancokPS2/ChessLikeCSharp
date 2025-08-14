using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.WorldMap;

[Obsolete("We redoin it")]
public partial class WorldMap3D : Node3D
{
	const string GROUP_TRAVEL_LOCATION_NODE = "group_travel_location";

	private List<WorldMapMarker3D> Markers { get => markers; set => Markers = value; }
	private List<WorldMapMarker3D> markers = new();
	private int MaxItemsSelected = 1;

	private void ClearMarkers()
	{
		List<WorldMapMarker3D> toDelete =
			GetTree().GetNodesInGroup(GROUP_TRAVEL_LOCATION_NODE)
			.OfType<WorldMapMarker3D>()
			.ToList();

		foreach (var item in toDelete)
			RemoveMarker(item);
	}

	public void AddMarker(WorldMapMarker3D element)
	{
		Markers.Add(element);
		AddChild(element);
		element.AddToGroup(GROUP_TRAVEL_LOCATION_NODE);
		ConnectMarker(element);
	}

	public void RemoveMarker(WorldMapMarker3D marker)
	{
		Markers.Remove(marker);
		marker.QueueFree();
	}

	public List<WorldMapMarker3D> GetSelectedMarkers()
		=> Markers.Where(x => x.Selected).ToList();

	public List<WorldMapMarker3D> GetHoveredMarkers()
		=> Markers.Where(x => x.Hovered).ToList();

	public List<WorldMapMarker3D> GetMarkers() => Markers;
	private void OnMarkerInput(Node camera, InputEvent input_event, Godot.Vector3 event_pos, Godot.Vector3 normal, long shape_idx, WorldMapMarker3D marker)
	{
		if (input_event.IsActionPressed(Global.GInput.GetActionName(Global.GInput.Button.ACCEPT)))
		{
			EventBus.MarkerSelected?.Invoke(marker);
		}
		else if (input_event is InputEventMouseMotion motion)
		{
			marker.Selected = true;
		}
	}

	private void OnMarkerHover(WorldMapMarker3D marker)
	{
		GetHoveredMarkers().ForEach(x => x.Hovered = false);
		marker.Hovered = true;
	}

    private void ConnectMarker(WorldMapMarker3D marker)
	{
		//Input
		Area3D area = marker.NodeArea; ;
		area.InputEvent +=
			(cam, eve, pos, nor, idx)
				=> OnMarkerInput(cam, eve, pos, nor, idx, marker);
		area.MouseEntered += () => OnMarkerHover(marker);
	}

}
