using ChessLike.Entity;
using Godot;

namespace ChessLike.World.Encounter;

public partial class SpawnSlot : Resource
{
    public Vector3i Location;
    public EFaction FactionAllowed;
    public SpawnSlot(Vector3i location, EFaction faction)
    {
        Location = location;
        FactionAllowed = faction;
    }   
}


