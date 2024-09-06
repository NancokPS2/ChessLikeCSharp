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
            encounter.PresetMobSpawns = new()
            {
            {
                Vector3i.ONE + Vector3i.RIGHT * 2, 
                new Mob()
                .ChainIdentity("SomeoneGuess", "Someone")
                .ChainJob(Prototypes.Jobs[EJob.CIVILIAN])
                .ChainResult()
            }  
            };

            return encounter;
        }
    }

}
