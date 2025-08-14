using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class TravelMap : Node3D
{
	public List<TravelMapLocation> Locations = new();

	public void RepopulateMap()
	{

		foreach (var item in Locations)
		{
			TravelLocationNodeAdd(item);
		}
	}

	public void TravelLocationNodeAdd(TravelMapLocation location)
	{
		WorldMapMarker3D node = GD.Load<PackedScene>("placeholder").Instantiate<WorldMapMarker3D>();
		node.SetResource(location);
		AddChild(node);

		//node.AddToGroup(GROUP_TRAVEL_LOCATION_NODE);
	}

}
