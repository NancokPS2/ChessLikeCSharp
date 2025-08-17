using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class WorldMap3D : Node3D
{
	const string GROUP_SELECTION_MARKER = "group_selection_marker";
	const string GROUP_TRAVEL_LOCATION_NODE = "group_travel_location";
	private int MaxItemsSelected = 1;

	private bool SelectionDirty = true;

	[Export]
	public PackedScene SelectedMarkerScene;

	public override void _EnterTree()
	{
		base._EnterTree();
		ChildEnteredTree += OnChildEnteredTree;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (SelectionDirty)
		{
			UpdateSelectionVisuals();
		}
	}

	private void UpdateSelectionVisuals()
	{
		SelectionDirty = false;
		foreach (var item in GetTree().GetNodesInGroup(GROUP_SELECTION_MARKER))
		{
			item.QueueFree();
		}

		foreach (var marker in GetSelectedMarkers())
		{
			Node3D newNode = SelectedMarkerScene.Instantiate<Node3D>();
			AddChild(newNode);
			newNode.GlobalPosition = marker.GlobalPosition + (Godot.Vector3.Up * 0.5f);
		}
	}

	public void ClearMarkers()
	{
		List<WorldMapMarker3D> toDelete = GetMarkers();

		foreach (var item in toDelete)
			RemoveMarker(item);
	}

	public void AddMarker(WorldMapMarker3D marker)
	{
		bool canShow = CanShowMarker(marker);

		if (!canShow)
		{
			marker.RemoveSelf();
			return;
		}

		if (!marker.IsInsideTree()) AddChild(marker);
		marker.AddToGroup(GROUP_TRAVEL_LOCATION_NODE);
		ConnectMarker(marker);
	}

	private bool CanShowMarker(WorldMapMarker3D marker)
	{
		if (marker.Location is null) return true;

		//Make sure that all the story flags have been met to show this location.
		foreach (var item in marker.Location.FlagWhitelist)
		{
			if (!SaveManager.IsStoryFlagSet(item)) return false;
		}

		foreach (var item in marker.Location.FlagBlacklist)
		{
			if (SaveManager.IsStoryFlagSet(item)) return false;
		}

		return true;
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
		List<WorldMapMarker3D> selectedMarkers = GetSelectedMarkers();
		bool alreadySelected = selectedMarkers.Contains(marker);

		if (alreadySelected)
		{
			EventBus.MapLocationConfirmed?.Invoke(marker.Location);
			marker.Select(false);
		}
		else
		{
			GetSelectedMarkers().ForEach(x => x.Select(false));
			marker.Select(true);
			EventBus.MapLocationSelected?.Invoke(marker.Location);
		}

		SelectionDirty = true;
	}

	private void OnMarkerHovered(WorldMapMarker3D marker)
	{
		GetHoveredMarkers().ForEach(x => x.Hover(false));
		marker.Hover(true);
	}

	private void OnChildEnteredTree(Node node)
	{
		if (node is WorldMapMarker3D marker)
			AddMarker(marker);
	}
	#endregion

}
