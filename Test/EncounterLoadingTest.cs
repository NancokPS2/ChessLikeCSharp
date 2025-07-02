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
    public GridNode gridNode = new();
    public EncounterData encounter = new();

    public override void _Ready()
    {
        base._Ready();

        encounter = EncounterData.GetDefault();

        EventBus.EncounterLoaded?.Invoke(encounter);

        foreach (var item in encounter.PresetMobSpawns)
        {
            item.Value.MobState = ChessLike.Entity.EMobState.COMBAT;
            item.Value.Move(new(item.Key));
        }
    }
}
