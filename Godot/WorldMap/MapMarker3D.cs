namespace Godot.WorldMap;

public partial class MapMarker3D : Node3D
{
    private EMapMarker type;
    private string displayedName = "Unnamed Marker";
    private Mesh? visualMesh = null;

    public EMapMarker Type { get => type; set => type = value; }

    public string DisplayedName { get => displayedName; set => displayedName = value; }

    public Mesh? VisualMesh { get => visualMesh; set => visualMesh = value; }

}