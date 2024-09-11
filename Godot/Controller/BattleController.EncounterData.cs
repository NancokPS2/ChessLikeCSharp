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
            Global.Setup();

            EncounterData encounter = new();
            encounter.Grid = Grid.Generator.GenerateFlat(new(6));
            encounter.SpawnLocations = new(){Vector3i.ONE};

            Mob def_mob = new Mob()
                .ChainName("SomeoneGuess")
                .ChainJob(Global.ManagerJob.GetAll()[0])
                .ChainResult();

            encounter.PresetMobSpawns = new()
            {
            {
                Vector3i.ONE + Vector3i.RIGHT * 2, 
                def_mob
            }  
            };

            return encounter;
        }
    }

}
