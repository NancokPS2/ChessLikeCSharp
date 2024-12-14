using ChessLike.Entity;
using ChessLike.World;

namespace ChessLike.World.Encounter;

public partial class EncounterData
{
    public Grid Grid = new();
    public Dictionary<Vector3i, List<EFaction>> FactionSpawns = new();
    public Dictionary<Vector3i, Mob> PresetMobSpawns = new();

    public static EncounterData GetDefault()
    {
        EncounterData encounter = new();
        encounter.Grid = Grid.Generator.GenerateFlat(new(6));
        encounter.FactionSpawns = new(){
            {Vector3i.ONE, new(){EFaction.PLAYER}}
            };

        Mob def_mob1 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("PlayerFac")
            .ChainFaction(EFaction.PLAYER)
            .ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));
            
        Mob def_mob2 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("PlayerFac2Warrior")
            .ChainFaction(EFaction.PLAYER)
            .ChainJob(new List<Job>(){Job.CreatePrototype(EJob.WARRIOR)})
            .ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));

        Mob def_mob3 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("Civilian")
            .ChainJob(new List<Job>(){Job.CreatePrototype(EJob.CIVILIAN)})
            .ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));

        Mob def_mob4 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainJob(new List<Job>(){Job.CreatePrototype(EJob.WIZARD)})
            .ChainName("Neutral wizard");

        encounter.PresetMobSpawns = new()
        {
        {
            Vector3i.UP, 
            def_mob1
        },
        {
            Vector3i.ONE + Vector3i.RIGHT * 2, 
            def_mob2
        },
        {
            Vector3i.UP + Vector3i.FORWARD * 2, 
            def_mob3
        },
        {
            Vector3i.UP + (Vector3i.RIGHT * 2) + (Vector3i.FORWARD * 2), 
            def_mob4
        }, 
        };

        return encounter;
    }

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
