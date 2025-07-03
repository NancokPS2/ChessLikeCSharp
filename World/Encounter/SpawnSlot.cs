using ChessLike.Entity;
using Godot;

namespace ChessLike.World.Encounter;

public partial class SpawnSlot : Resource
{
    public Vector3i Location;
    [Export]
    private Vector3I location
    {
        set => Location = new(value);
        get => Location.ToGVector3I();
    }

    [Export]
    public EFaction FactionAllowed = EFaction.PLAYER;

    [Export]
    public Mob? PresetMob;
}


