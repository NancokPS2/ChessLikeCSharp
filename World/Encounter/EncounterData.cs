using ChessLike.Entity;
using ChessLike.Shared.Storage;
using ChessLike.World;
using Godot;

namespace ChessLike.World.Encounter;

[GlobalClass]
public partial class EncounterData : Resource
{
    public Grid Grid = new();

    public List<SpawnSlot> MobPlacement = new();
    [Export]
    private Godot.Collections.Array<SpawnSlot> mobPlacement
    {
        set => MobPlacement = new(value);
        get => new(MobPlacement);
    }

    public int RoundLimit = -1;

    int RoundCount;

    public EncounterData()
    {
        EventBus.RoundEnded += () => RoundCount++;
    }

    public virtual void EncounterProcess()
    {

    }

    public virtual bool IsFinished()
    {
        bool no_hostiles_remaining = !Global.ManagerMob
            .GetPooledInCombat()
            .Any(x => x.GetFaction().IsEnemy(EFaction.PLAYER));

        bool turn_limit_reached = RoundLimit > 0 && RoundCount >= RoundLimit;

        return no_hostiles_remaining || turn_limit_reached;
    }

    public static EncounterData GetDefault()
    {
        EncounterData encounter = new();
        encounter.Grid = GridTerrainGenerator.GenerateFlat(new(6));

        Mob def_mob1 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("PlayerFac")
            .ChainFaction(EFaction.PLAYER);
        def_mob1.Move(Vector3i.ONE);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));

        Mob def_mob2 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("PlayerFac2Warrior")
            .ChainFaction(EFaction.PLAYER)
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.WARRIOR) });
        def_mob2.Move(Vector3i.ONE+Vector3i.FORWARD);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));

        Mob def_mob3 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("Civilian")
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.CIVILIAN) });
        def_mob3.Move(Vector3i.ONE+Vector3i.LEFT);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Shared.Storage.EItem.SWORD));

        Mob def_mob4 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.WIZARD) })
            .ChainName("Neutral wizard");
        def_mob4.Move(Vector3i.ONE+Vector3i.FORWARD+Vector3i.FORWARD);
        //.ChainEquipment(new WeaponSpear());

        encounter.MobPlacement = new(){
                new(){
                    Location = new(0,1,0),
                    FactionAllowed = EFaction.PLAYER,
                    PresetMob = def_mob1
                },
                new(){
                    Location = new(2,1,2),
                    FactionAllowed = EFaction.PLAYER,
                    PresetMob = def_mob2
                },
                new(){
                    Location = new(2,1,1),
                    FactionAllowed = EFaction.PLAYER,
                    PresetMob = def_mob3
                },
                new(){
                    Location = new(0,1,2),
                    FactionAllowed = EFaction.PLAYER,
                    PresetMob = def_mob4
                }
            };

        return encounter;
    }

    public virtual void Finish()
    {

    }

}
