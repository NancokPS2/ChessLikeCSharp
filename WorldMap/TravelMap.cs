using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class TravelMap : Node3D
{
	const string GROUP_TRAVEL_LOCATION_NODE = "group_travel_location";
	public List<TravelMapLocation> Locations = new();

	public void RepopulateMap()
	{
		TravelLocationClear();

		foreach (var item in Locations)
		{
			TravelLocationNodeAdd(item);
		}
	}

	public void TravelLocationNodeAdd(TravelMapLocation location)
	{
		MapMarker3D node = GD.Load<PackedScene>("placeholder").Instantiate<MapMarker3D>();
		node.SetResource(location);
		AddChild(node);

		node.AddToGroup(GROUP_TRAVEL_LOCATION_NODE);
	}

	public void TravelLocationClear()
	{
		foreach (var item in GetTree().GetNodesInGroup(GROUP_TRAVEL_LOCATION_NODE))
			item.QueueFree();
	}
}
