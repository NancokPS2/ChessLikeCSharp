

using ChessLike.Entity;
using ChessLike.World;

using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node
{
    private readonly Mesh MESH_TARGETING = new CylinderMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(0,0,1, 0.5f)}
    };

    private readonly Mesh MESH_AOE = new SphereMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,0,0)}
    };

    private readonly Mesh MESH_OTHER = new PrismMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,1,0)}
    };

    private Label debug_info_label = new();

    public EncounterData encounter;
    public Grid grid;
    public Action action_selected;
    public Vector3i position_selected;
    public Action.UsageParams usage_params_in_construction;
    public Mob mob_taking_turn;

    public override void _Ready()
    {
        base._Ready();
        
        LoadEncounter(EncounterData.GetDefault());
        AddChild(debug_info_label);
        PrintTreePretty();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateDebugInformation();

    }

    public void LoadEncounter(EncounterData to_load)
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        grid = to_load.Grid;
        encounter = to_load;

        SetupDisplay();
    }

    public void UpdateDebugInformation()
    {
        debug_info_label.Text = string.Format(
            "State: {0}" + "\n" +
            "Action selected: {1}" + "\n" +
            "Grid size: {2}", 
            new object[]{
                state_current, 
                action_selected != null ? action_selected.name : "null", 
                grid != null ? grid.boundary : "null"}
            
            );
    }

}
