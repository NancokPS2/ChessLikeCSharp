

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
    public Vector3i PositionHovered;
    public Vector3i PositionSelected;
    public Action.UsageParams UsageParameters;
    public List<Mob> mobs_participating = new();
    public Mob mob_taking_turn;
    public float delay_this_turn;
    public float StateTimeWithoutChange;

    public override void _Ready()
    {
        base._Ready();
        Global.GInput.ConnectToWindow(GetWindow());

        LoadEncounter(EncounterData.GetDefault());
        AddChild(debug_info_label);
        SetState(StateCurrent);
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
        
        //Add the mobs after setting their position.
        foreach (var item in encounter.PresetMobSpawns)
        {
            item.Value.Position = item.Key;
            AddParticipant(item.Value);
        }
    }

    public void AddParticipant(Mob mob)
    {
        mobs_participating.Add(mob);
        display_mob.Add(mob);
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
                StateCurrent, 
                action_selected != null ? action_selected.name : "null", 
                grid != null ? grid.Boundary : "null",
                mob_taking_turn != null ? mob_taking_turn.DisplayedName : "null",
                PositionHovered,
                display_camera != null ? display_camera.Rotation : "???"
                }
            
            );
    }

}
