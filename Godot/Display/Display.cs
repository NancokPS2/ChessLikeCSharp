using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot;

public partial class Display : Godot.Node
{
    public MobDisplay mob_display = new();



    public override void _Ready()
    {
        base._Ready();
        AddChild(mob_display);
        TestMovement();

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        mob_display.UpdateMeshPositions();
    }

    public void TestMovement()
    {
        mob_display.AddMob(
            new ChessLike.Entity.Mob(){Position = new Vector3i(1,1,1)}
            );
        mob_display.AddMob(
            new ChessLike.Entity.Mob(){Position = new Vector3i(2,2,2)}.PresetDefaultStats()
            );
        mob_display.AddMob(
            new ChessLike.Entity.Mob(){Position = new Vector3i(3,3,3)}.PresetCriticalHealth()
            );

        GD.Print(mob_display.GetChildren());

        Timer step = new Timer(){Autostart = true, WaitTime = 1};
        step.Timeout += MoveMobRandom;
        AddChild(step);

    }

    public void MoveMobRandom()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        
        int index = rng.RandiRange(0, Vector3i.DIRECTIONS.Length-1);
        Vector3i direction = Vector3i.DIRECTIONS[index];

        index = rng.RandiRange(0, mob_display.GetMobs().Count-1);
        Mob mob = mob_display.GetMobs()[index];

        mob.Position += direction;

    }
}
