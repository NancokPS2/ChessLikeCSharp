using ChessLike.Entity;
using ChessLike.World;

namespace Godot;

public partial class EncounterDataResource : Resource
{
    [Export]
    public GridResource Grid = new();
    [Export]
    public Collections.Dictionary<Vector3I, Collections.Array<EFaction>> FactionSpawns = new();
    [Export]
    public Collections.Dictionary<Vector3I, MobResource> PresetMobSpawns = new();
    [Export]
    public int RoundLimit = 0;
}
