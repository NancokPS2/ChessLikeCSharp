

using ChessLike.Entity;
using ChessLike.Shared.DebugDisplay;
using ChessLike.World;
using Godot.Display;

using Action = ChessLike.Entity.Action;

namespace Godot;

[GlobalClass]
public partial class BattleController : Node, IDebugDisplay
{
    public override void _Ready()
    {
        base._Ready();
        Global.ConnectToWindow(GetWindow());
        DebugDisplay.Instance.Add(this);

        SetupEncounter(EncounterData.GetDefault());

        SetState(StateCurrent);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        ProcessStateInput();
        ProcessState(delta);

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
        string output = string.Format(
            "State: {0}" + "\n" +
            "Action selected: {1}" + "\n" +
            "Grid size: {2}" + "\n" +
            "Unit selected: {3}" + "\n" +
            "Location selected: {4}" + "\n" + 
            "Camera rotation: {5}",
            new object[]{
                StateCurrent, 
                InputActionSelected != null ? InputActionSelected.Name : "null", 
                CompGrid != null ? CompGrid.Boundary : "null",
                (CompTurnManager.GetCurrentTurnTaker() as Mob) != null ? (CompTurnManager.GetCurrentTurnTaker() as Mob).DisplayedName : "null",
                PositionHovered,
                CompCamera != null ? CompCamera.Rotation : "???"
                }
            
        );
        return output;
    }

}
