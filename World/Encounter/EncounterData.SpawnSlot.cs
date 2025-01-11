using ChessLike.Entity;

namespace ChessLike.World.Encounter;

public partial class EncounterData
{
    public struct SpawnSlot
    {
        public Vector3i Location;
        public EFaction FactionAllowed;
        public SpawnSlot(Vector3i location, EFaction faction)
        {
            Location = location;
            FactionAllowed = faction;
        }   
    }

}
