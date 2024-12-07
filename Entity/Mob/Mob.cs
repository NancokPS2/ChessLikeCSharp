using ChessLike.Entity.Action;
using ChessLike.Entity.Command;
using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Entity;


public partial class Mob
{
    public string DisplayedName = "UNNAMED";
    private List<Job> Jobs = new(){Job.CreatePrototype(EJob.DEFAULT)};
    private List<Ability> Actions = new();
    private List<Passive> Passives = new();
    public ERace Race = ERace.HUMAN;
    public EFaction Faction = EFaction.NEUTRAL;
    public Inventory MobInventory = new();
    private EMovementMode _movement_mode;
    public EMovementMode MovementMode {set => SetMovementMode(value); get => _movement_mode;}
    public EMobState MobState = EMobState.BENCHED;
    public MobStatSet Stats = GetDefaultStats();

    public Vector3i Position;

    public Mob()
    {
        //TODO: Move this somewhere else
        Global.ManagerMob.AddPooled(this);

        //Default stats
        Stats = GetDefaultStats();

        //Default inventory
        Inventory inv = Inventory.FromResource(Inventory.LoadPreset(Inventory.EPreset.EQUIPMENT));
        MobInventory = inv;

    }

    public List<Job> GetJobs()
    {
        return Jobs;
    }

    private void UpdateJobs()
    {
        if (Jobs.Count == 0)
        {
            throw new Exception("No jobs defined.");
        }

        //Reset stats and update them.
        Stats = Jobs.First().Stats;

        foreach (Job job in Jobs)
        {
            //Average the stats from the job's.
            Stats = new(MobStatSet.GetAverage(Stats, job.Stats));

            //Add the actions.
            Actions.AddRange(job.Abilities);

            //TODO: Make the selected mode be deterministic instead of selecting the last job of the list.
            SetMovementMode(job.MovementMode);
        }
    }

    public static MobStatSet GetDefaultStats()
    {
        MobStatSet output = new();
        output.SetStat(StatName.HEALTH, 100);
        output.SetStat(StatName.ENERGY, 30);
        output.SetStat(StatName.AGILITY, 100);
        output.SetStat(StatName.STRENGTH, 100);
        output.SetStat(StatName.INTELLIGENCE, 100);
        output.SetStat(StatName.MOVEMENT, 3);
        output.SetStat(StatName.JUMP, 2);
        output.SetStat(StatName.DELAY, 100);
        return output;
    }

}