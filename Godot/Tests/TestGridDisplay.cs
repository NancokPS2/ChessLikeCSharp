using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using ExtendedXmlSerializer;

namespace Godot;

[GlobalClass]
public partial class TestGridDisplay : Node
{
    public Grid grid;
    public GridNode Display = new GridNode();
    public Mob MobTest;

    //Buttons
    [Export]
    public Button? TestPathfinding1 = new();
    [Export]
    public Button? TestPathfinding2 = new();

    public TestGridDisplay()
    {
        grid = Grid.Generator.GenerateFlat(new(6));
        Display.SetGrid(grid);
        MobTest = Mob.CreatePrototype(EMobPrototype.HUMAN);
        MobTest.Position = new(2,1,2);
        MobTest.Stats.SetStat(StatName.MOVEMENT, 2);
        MobTest.Stats.SetStat(StatName.JUMP, 2);
    }

    public override void _Ready()
    {
        base._Ready();
        AddChild(Display);

        List<Vector3i> cells_to_mark = grid.NavGetPathablePositions(MobTest);
        cells_to_mark.ForEach(x => GD.Print(x));
        cells_to_mark.ForEach(
            x => Display.MeshSet(
                x,
                GridNode.Layer.TARGETING,
                new SphereMesh()
            )
        );


        TestPathfinding1.Pressed += () => OnPressed(TestPathfinding1);
        TestPathfinding2.Pressed += () => OnPressed(TestPathfinding2);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
    }

    public void OnPressed(Button button)
    {
        if (button == TestPathfinding1)
        {
        }
    }
}
