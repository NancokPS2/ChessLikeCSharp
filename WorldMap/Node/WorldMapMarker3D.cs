using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class WorldMapMarker3D : Node3D, ISelectable
{
	public delegate void MarkerEvent(WorldMapMarker3D marker);
	public event MarkerEvent MarkerSelected;
	public event MarkerEvent MarkerHovered;
	public string DisplayedName
	{
		get => displayedName;
		set
		{
			displayedName = value;
			if (IsInsideTree()) UpdateLabel();
		}
	}
	private string displayedName = "Unnamed Marker";

	public EMapMarker Type { get; set; }
	public bool Selected { get; set; }
	public bool Hovered { get; set; }

	[Export]
	public TravelMapLocation? Location;

	[Export]
	public Label3D LabelNode;
	[Export]
	public Area3D AreaNode;

	public WorldMapMarker3D()
	{

	}

	public override void _Ready()
	{
		base._Ready();
		AreaNode.InputEvent += OnAreaInputEvent;

		UpdateLabel();
	}

	public void Select(bool select)
	{
		Selected = select;
		UpdateLabel();
	}

	public void Hover(bool hover)
	{
		Hovered = hover;
		UpdateLabel();
	}

	private SphereShape3D GenerateCollisionShape(MeshInstance3D meshInst)
	{
		Aabb mesh_aabb = meshInst.GetAabb();
		return new SphereShape3D() { Radius = mesh_aabb.Size.X / 2 };
	}

	private void UpdateLabel()
	{
		LabelNode.Text = DisplayedName;

		if (Selected)
		{
			LabelNode.Modulate = LabelNode.Modulate = Colors.Green;
		}
		else if (Hovered)
		{
			LabelNode.Modulate = LabelNode.Modulate = Colors.Yellow;
		}
		else
		{
			LabelNode.Modulate = LabelNode.Modulate = Colors.White;
		}
	}

	public void SetResource(TravelMapLocation location)
	{
		Location = location;
	}

	#region Event Handling
	private void OnAreaInputEvent(Node camera, InputEvent @event, Godot.Vector3 eventPosition, Godot.Vector3 normal, long shapeIdx)
	{
		if (@event.IsActionPressed(Readonly.InputActions.PRIMARY))
		{
			MarkerSelected?.Invoke(this);
		}
		else if (@event is InputEventMouseMotion)
		{
			MarkerHovered?.Invoke(this);
		}
	}
	#endregion
}