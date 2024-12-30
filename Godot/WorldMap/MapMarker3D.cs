namespace Godot.WorldMap;

public partial class MapMarker3D : Node3D, ISelectable
{
    private EMapMarker type;
    private string displayedName = "Unnamed Marker";
    private Mesh? visualMesh = null;

    private bool selected;

    public EMapMarker Type { get => type; set => type = value; }

    public string DisplayedName { get => displayedName; set => displayedName = value; }

    public Mesh? VisualMesh { get => visualMesh; set => visualMesh = value; }
    public bool Selected { get => selected; set => selected = value; }

    public event ISelectable.SelectionChange? SelectedEvent;
}