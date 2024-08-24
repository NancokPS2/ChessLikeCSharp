using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using Godot;

namespace ChessLike.Godot.Tests;

[GlobalClass]
public partial class GridNavigationTest : Node
{
    public override void _Ready()
    {
        base._Ready();
        NavigationTest(new(3,1,2));
        NavigationTest(new(3,0,2));
        NavigationTest(new(1,0,2));
        NavigationTest(new(1,0,0));
    }

    public void NavigationTest(Vector3i target)
    {
        Mob mob = new();
        Grid grid = Grid.Generator.GenerateFlat(new(10));
        Navigation navigation = new(grid);

        NavigationAgent agent = navigation.AddAgent(mob);

        var path = navigation.ConstructPathToLocation(agent, 10, target);
        Debug.Assert(path.Count == agent.user.Position.DistanceManhattanTo(target), agent.user.Position.DistanceManhattanTo(target).ToString());

        if(path.Count == agent.user.Position.DistanceManhattanTo(target))
        {
            Console.WriteLine("Test passed.");
        }else
        {
            Console.WriteLine("Test failed.");
            foreach (var item in path)
            {
                Console.Write(item);
            }
        }
    }
}
