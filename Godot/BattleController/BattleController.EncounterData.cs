using ChessLike.Entity;
using ChessLike.World;

namespace Godot;

public partial class BattleController
{
    public class EncounterData
    {
        public Grid Grid = new();
        public List<Vector3i> SpawnLocations = new();
        public Dictionary<Vector3i, Mob> PresetMobSpawns = new();

        public static EncounterData GetDefault()
        {


            EncounterData encounter = new();
            encounter.Grid = Grid.Generator.GenerateFlat(new(6));
            encounter.SpawnLocations = new(){Vector3i.ONE};

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
    }

}
