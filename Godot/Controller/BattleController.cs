

using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;

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

    private readonly Mesh MESH_CURSOR = new PrismMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,0,0)},
        Size = new Godot.Vector3(1,2,1),
        
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
    public List<Mob> mobs_participating = new();
    public Mob mob_taking_turn;
    public float delay_this_turn;
    public float time_passed_this_state;

    public override void _Ready()
    {
        base._Ready();
        
        LoadEncounter(EncounterData.GetDefault());
        AddChild(debug_info_label);
        SetState(state_current);
        PrintTreePretty();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateDebugInformation();
        ProcessStateInput();
        ProcessState(delta);

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
        
        foreach (Mob mob in encounter.PresetMobSpawns.Values)
        {
            AddParticipant(mob);
        }
    }

    public void AddParticipant(Mob mob)
    {
        mobs_participating.Add(mob);
        display_mob.Add(mob);
        display_mob.SetMobInUI(mob);
    }

    public void UpdateDebugInformation()
    {
        debug_info_label.Text = string.Format(
            "State: {0}" + "\n" +
            "Action selected: {1}" + "\n" +
            "Grid size: {2}" + "\n" +
            "Unit selected: {3}" + "\n" +
            "Location selected: {4}" + "\n" + 
            "Camera rotation: {5}",
            new object[]{
                state_current, 
                action_selected != null ? action_selected.name : "null", 
                grid != null ? grid.boundary : "null",
                mob_taking_turn != null ? mob_taking_turn.Identity.Name : "null",
                position_selected,
                display_camera != null ? display_camera.Rotation : "???"
                }
            
            );
    }

}
