using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.World.Encounter;

public partial class EncounterData : IResourceSerialize<EncounterData, EncounterDataResource>
{
    public EncounterDataResource ToResource()
    {
        EncounterDataResource output = new();
        output.Grid = Grid.ToResource();
        output.FactionSpawns = new();
        foreach (var item in FactionSpawns)
        {
            output.FactionSpawns[item.Key.ToGVector3I()] = new(item.Value);
        }
        foreach (var item in PresetMobSpawns)
        {
            output.PresetMobSpawns[item.Key.ToGVector3I()] = item.Value.ToResource();
        }
        return output;
    }

    public static EncounterData FromResource(EncounterDataResource res)
    {
        EncounterData output = new();
        output.Grid = Grid.FromResource(res.Grid);
        foreach (var item in res.FactionSpawns)
        {
            output.FactionSpawns[new(item.Key)] = new(item.Value);
        }
        foreach (var item in res.PresetMobSpawns)
        {
            output.PresetMobSpawns[new(item.Key)] = Entity.Mob.FromResource(item.Value);
        }
        return output;
    }
}
