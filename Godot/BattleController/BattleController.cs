

using ChessLike.Entity;
using ChessLike.World;
using Godot.Display;

using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node
{
    public override void _Ready()
    {
        base._Ready();
        Global.ConnectToWindow(GetWindow());

        SetupEncounter(EncounterData.GetDefault());

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
                TurnActionSelected != null ? TurnActionSelected.name : "null", 
                CompGrid != null ? CompGrid.Boundary : "null",
                (CompTurnManager.GetCurrentTurnTaker() as Mob) != null ? (CompTurnManager.GetCurrentTurnTaker() as Mob).DisplayedName : "null",
                PositionHovered,
                CompCamera != null ? CompCamera.Rotation : "???"
                }
            
            );
    }

}
