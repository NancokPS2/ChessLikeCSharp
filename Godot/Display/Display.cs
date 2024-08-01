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
    public Camera camera = new();



    public override void _Ready()
    {
        base._Ready();

        AddChild(mob_display);

        AddChild(camera);
        camera.Position = new(0,1,1);
        camera.Watch();

        TestMovement();

    }

    public void TestMovement()
    {
        mob_display.AddMob(
            new ChessLike.Entity.Mob.Builder()
                .SetPosition(new Vector3i(1))
                .SetIdentity("identified", "Jhonny")
                .SetJob(Job.Preset.Warrior())
                .Update()
                .Result()
            );
        mob_display.AddMob(
            new ChessLike.Entity.Mob.Builder()
                .SetPosition(new Vector3i(2))
                .SetJob(Job.Preset.Grunt())
                .SetIdentity("identified2", "Potty")
                .Update()
                .Result()
            );
        mob_display.AddMob(
            new ChessLike.Entity.Mob.Builder()
                .SetPosition(new Vector3i(3))
                .SetJob(new[]{Job.Preset.Medic(), Job.Preset.Grunt()})
                .SetIdentity("identified", "Clone J")
                .Update()
                .Result()
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
