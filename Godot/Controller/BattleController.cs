

using ChessLike.World;
using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node
{
    private Label debug_info_label = new();

    public EncounterData encounter;
    public Grid grid;
    public Action selected_action;

    public enum State
    {
        PAUSED,
        AWAITING_ACTION,
    }

    public State state_current = State.PAUSED;

    public override void _Ready()
    {
        base._Ready();
        
        AddChild(debug_info_label);
        LoadEncounter(EncounterData.GetDefault());

        SetupDisplay();
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
                selected_action != null ? selected_action.name : "null", 
                grid != null ? grid.boundary : "null"}
            
            );
    }

}
