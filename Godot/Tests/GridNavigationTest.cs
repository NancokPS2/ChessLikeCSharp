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
        Grid grid = Grid.Generator.GenerateFlat(new(4));
        Navigation navigation = new(grid);

        NavigationAgent agent = navigation.AddAgent(mob);

        List<Vector3i> valid_pos = navigation.GetPositionsInRange(agent, 10);
        List<Vector3i> path = Navigation.GetShortestPath(valid_pos, agent.user.Position, target);

        //Debug.Assert(path.Count == agent.user.Position.DistanceManhattanTo(target), (agent.user.Position.DistanceManhattanTo(target)-1).ToString());

        //The path includes the starting position, so it is reduced to -1
        int dist_to_target = agent.user.Position.DistanceManhattanTo(target);
        if(path.Count == dist_to_target + 1 || path.Count == 0)
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
