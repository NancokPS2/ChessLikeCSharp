namespace Godot.WorldMap;

public partial class MapMarker3D : Node3D, ISelectable
{
    private string displayedName = "Unnamed Marker";

    public EMapMarker Type { get;set; }
    public bool Selected { get;set; }
    public bool Hovered { get;set; }

    public string DisplayedName
    {
        get => displayedName; 
        set
        {
            NodeLabel.Text = value;
            displayedName = value;
        }
    }

    public MeshInstance3D NodeMesh { get;set; } = new();
    public Label3D NodeLabel {get;set;} = new();
    public Area3D NodeArea {get;set;} = new(){InputRayPickable = true};
    public CollisionShape3D NodeCollision = new();

    public MapMarker3D()
    {
        
    }

    public List<Node> GetNodes() => new(){NodeMesh, NodeLabel, NodeArea};

    public override void _EnterTree()
    {
        base._EnterTree();
        GetNodes().ForEach(x => AddChild(x));
        NodeArea.AddChild(NodeCollision);

        UpdateNodes();
    }

    public void UpdateNodes()
    {
        UpdateCollision();
        UpdateLabel();
    }

    private void UpdateCollision()
    {
        Aabb mesh_aabb = NodeMesh.GetAabb();
        NodeCollision.Shape = new SphereShape3D() { Radius = mesh_aabb.Size.X / 2 };
    }

    private void UpdateLabel()
    {
        NodeLabel.Text = DisplayedName;
        Aabb mesh_aabb = NodeMesh.GetAabb();
        NodeLabel.Position = new(0, -mesh_aabb.End.Y , 0);

        if (Selected)
        {
            NodeLabel.Modulate = NodeLabel.Modulate = Colors.Green;
        }
        else if (Hovered)
        {
            NodeLabel.Modulate = NodeLabel.Modulate = Colors.Yellow;
        }
        else
        {
            NodeLabel.Modulate = NodeLabel.Modulate = Colors.White;
        }
    }
}