

using ChessLike.Entity;
using ChessLike.Shared.DebugDisplay;
using ChessLike.Turn;
using ChessLike.World;
using Godot.Display;

using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node, IDebugDisplay
{

    private bool _ready_for_debug;
    public override void _Ready()
    {
        base._Ready();
        Global.ConnectToWindow(GetWindow());
        DebugDisplay.Instance.Add(this);

        FSMSetup();
        SetupEncounter(EncounterData.GetDefault());
        //FSMSetState(State.TAKING_TURN);

        _ready_for_debug = true;

    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        FSMProcess(delta);

        Testing();

    }


    public void Testing()
    {
        if (Input.IsActionJustPressed("debug_draw"))
        {
            Mob mob = Global.ManagerMob.GetAll()[0];

            mob.Stats.SetValue(StatName.HEALTH, mob.Stats.GetValue(StatName.HEALTH) + 5);
        }
    }

    public string GetText()
    {
        if (!_ready_for_debug){return "";}

        string output = string.Format(
            "State: {0}" + "\n" +
            "Action selected: {1}" + "\n" +
            "Grid size: {2}" + "\n" +
            "Unit selected: {3}" + "\n" +
            "Location selected: {4}" + "\n" + 
            "Camera rotation: {5}" + "\n" +
            "Unit taking turn: {6}"
            ,
            new object[]{
                StateCurrent is not null ? StateCurrent.StateIdentifier : "null", 
                InputActionSelected is not null ? InputActionSelected.Name : "null", 
                CompGrid != null ? CompGrid.Boundary : "null",
                (CompTurnManager.GetCurrentTurnTaker() as Mob) != null ? (CompTurnManager.GetCurrentTurnTaker() as Mob).DisplayedName : "null",
                PositionHovered,
                CompCamera != null ? CompCamera.Rotation : "???",
                CompTurnManager is not null ? CompTurnManager.GetCurrentTurnTaker() : "null",
                }
            
        );
        return output;
    }

}
