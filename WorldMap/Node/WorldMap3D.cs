using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class WorldMap3D : Node3D
{
	const string GROUP_TRAVEL_LOCATION_NODE = "group_travel_location";
	private int MaxItemsSelected = 1;

	private void ClearMarkers()
	{
		List<WorldMapMarker3D> toDelete = GetMarkers();

		foreach (var item in toDelete)
			RemoveMarker(item);
	}

	public void AddMarker(WorldMapMarker3D element)
	{
		if (!element.IsInsideTree()) AddChild(element);
		element.AddToGroup(GROUP_TRAVEL_LOCATION_NODE);
		ConnectMarker(element);
	}

	private void ConnectMarker(WorldMapMarker3D element)
	{
		element.MarkerSelected += OnMarkerSelected;
		element.MarkerHovered += OnMarkerHovered;
	}


	public void RemoveMarker(WorldMapMarker3D marker)
	{
		marker.QueueFree();
	}

	public List<WorldMapMarker3D> GetSelectedMarkers()
		=> GetMarkers().Where(x => x.Selected).ToList();

	public List<WorldMapMarker3D> GetHoveredMarkers()
		=> GetMarkers().Where(x => x.Hovered).ToList();

	public List<WorldMapMarker3D> GetMarkers()
		=> GetTree().GetNodesInGroup(GROUP_TRAVEL_LOCATION_NODE)
			.OfType<WorldMapMarker3D>()
			.ToList();

	#region Event Handling
	private void OnMarkerSelected(WorldMapMarker3D marker)
	{
		GetSelectedMarkers().ForEach(x => x.Select(false));
		marker.Select(true);
	}

	private void OnMarkerHovered(WorldMapMarker3D marker)
	{
		GetHoveredMarkers().ForEach(x => x.Hover(false));
		marker.Hover(true);
	}
	#endregion

}
