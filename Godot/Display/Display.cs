using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike;
using ChessLike.Entity;
using ChessLike.World;

namespace Godot.Display;

public partial class Display : Godot.Node
{
    public MobDisplay mob_display = new();
    public GridDisplay grid_display = new();
    public Camera camera = new();



    public override void _Ready()
    {
        base._Ready();

        AddChild(mob_display);
        AddChild(grid_display);

        AddChild(camera);
        camera.Position = new(0,1,1);
        camera.Watch();

        TestVectors();
        TestMovement();
        TestGrid();

    }

    public void TestGrid()
    {
        grid_display.LoadGrid(Grid.Generator.GenerateFlat(new(10)));

    }

    public void TestVectors()
    {
        Vector3i a = new(0);
        Vector3i b = new(-1,-5,20);

        List<Vector3i> output = a.GetStepsToReachVector(b);
        bool A = true;

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
        mob_display.AddMob(
            new ChessLike.Entity.Mob.Builder()
                .SetPosition(new Vector3i(3))
                .SetJob(new[]{Job.Preset.Medic(), Job.Preset.Grunt()})
                .SetIdentity("identified", "Presetted")
                .Update()
                .Result()
            );

        //Test with a serialized Mob.
        string test_path = Path.Combine(Global.Directory.GetContentDir(Global.Directory.Content.USER_CONTENT) + @"\new.txt");
        Serializer.SaveAsXml(mob_display.GetMobs()[0], test_path);
        Mob serialized_mob = Serializer.LoadAsXml<Mob>(test_path);
        serialized_mob.Identity.Name = "PERMANENT";
        mob_display.AddMob(serialized_mob);

        mob_display.SetMobInUI(mob_display.GetMobs()[0]);

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
