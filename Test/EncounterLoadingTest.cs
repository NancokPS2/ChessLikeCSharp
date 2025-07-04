using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot;

namespace Tests;

public partial class EncounterLoadingTest : Node3D
{
    public override void _Ready()
    {
        base._Ready();
        CombatScene scene = Readonly.Scenes.MAIN_COMBAT;
        AddChild(scene);
        scene.Setup(EncounterData.GetDefault());
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventKey key && @event.IsReleased() && key.Keycode == Key.Space)
        {
            TestMobMovement();
        }
        else
        {
            TestMobInput();
        }
    }

    public static void TestMobInput()
    {
        Godot.Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3i direction = new((int)float.Round(inputDirection.X), 0, (int)float.Round(inputDirection.Y));
        Mob mob = Global.ManagerMob.GetPooledInCombat()[0];
        mob.MoveRelative(direction);
    }

    private static void TestMobMovement()
    {
        Mob mob = Global.ManagerMob.GetPooledInCombat()[0];

        Vector3i initialPos = mob.GetPosition();
        List<Vector3i> positions = new();
        Vector3i tempPos = initialPos;
        for (int i = 0; i < 4; i++)
        {
            tempPos += Vector3i.FORWARD;
            positions.Add(tempPos);
        }
        for (int i = 0; i < 4; i++)
        {
            tempPos += Vector3i.BACK;
            positions.Add(tempPos);
        }
        mob.MoveTroughPath(positions);

        Debug.Assert(initialPos == mob.GetPosition());
    }
}
