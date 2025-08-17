using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using ChessLike.WorldMap;
using Godot;

[GlobalClass, Tool]
public partial class EncounterBuilder : Node3D
{

    [Export]
    protected GridNode GridNodeUsed;

[ExportCategory("Grid")]
    protected Grid GridLoaded;
    [Export]
    public Grid GridToLoad;

    [ExportToolButton("Load Grid")]
    public Callable GridLoadCall
    {
        get => Callable.From(GridLoad);
    }

    private void GridLoad()
    {
        GridLoaded = GridToLoad;
        GridNodeUsed.SetGrid(GridLoaded);
    }

    [ExportCategory("Encounter")]
    [Export]
    protected EncounterData EncounterLoaded = new();

    [Export(PropertyHint.SaveFile, "*.tres")]
    protected string EncounterSavePath = "user://SavedEncounter.tres";

    [ExportCategory("Mob Placement")]

    protected Dictionary<Vector3i, Mob> MobPlacementDictionary = new();
    [Export]
    private Godot.Collections.Dictionary<Vector3I, Mob> mobPlacementDictionary
    {
        set
        {
            MobPlacementDictionary = value
                .ToDictionary(
                    x => new Vector3i(x.Key),
                    y => y.Value
                );
        }
        get
        {
            return new(MobPlacementDictionary
                .ToDictionary(
                    x => x.Key.ToGVector3I(),
                    y => y.Value
                )
            );
        }
    }

    [Export]
    protected Vector3I MobPlacementPosition;

    [Export]
    protected Mob? MobPlacementPresetMob;

    [ExportToolButton("Add MobPlacement")]
    protected Callable MobPlacementAddCall
    {
        get => Callable.From(MobPlacementAdd);
    }
    private void MobPlacementAdd()
    {
        Vector3i vector = new(MobPlacementPosition);
        if (MobPlacementDictionary.ContainsKey(vector))
        {
            GD.Print($"Cannot add, there is already a Mob at {MobPlacementPosition}, remove it first.");
            return;
        }
        if (MobPlacementPresetMob is null)
        {
            GD.Print($"No mob was selected.");
            return;
        }

        MobPlacementDictionary[vector] = MobPlacementPresetMob;
    }

    /* [ExportCategory("Spawn Slots")]
    protected Dictionary<Vector3i, SpawnSlot> SpawnSlots = new();
    [Export]
    private Godot.Collections.Dictionary<Vector3I, SpawnSlot> spawnSlots
    {
        set
        {
            SpawnSlots = value
                .ToDictionary(
                    x => new Vector3i(x.Key),
                    y => y.Value
                );
        }
        get
        {
            return new(SpawnSlots
                .ToDictionary(
                    x => x.Key.ToGVector3I(),
                    y => y.Value
                )
            );
        }
    }
 */

}
