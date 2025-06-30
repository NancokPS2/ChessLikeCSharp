using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot;

namespace Tests;

public partial class EncounterLoadingTest : Node3D
{
    public Grid grid = new();
    public EncounterData encounter = new();

    public override void _Ready()
    {
        base._Ready();

        
    }
}
