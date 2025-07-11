using ChessLike.Entity;
using ChessLike.Storage;
using ChessLike.World;
using Godot;

namespace ChessLike.World.Encounter;

[GlobalClass]
public partial class EncounterData : Resource
{
    [Export]
    public Grid Grid = new();

    public Dictionary<Vector3i, Mob> MobPlacement = new();
    [Export]
    private Godot.Collections.Dictionary<Godot.Vector3I, Mob> mobPlacement
    {
        set
        {
            MobPlacement = value
                .ToDictionary(
                    x => new Vector3i(x.Key),
                    y => y.Value
                    );
        }
        get
        {
            return new(
                MobPlacement
                .ToDictionary(
                    x => x.Key.ToGVector3I(),
                    y => y.Value
                    )
                );
        }

    }

    public int RoundLimit = -1;

    int RoundCount;

    public EncounterData()
    {
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
            .ChainAction(Global.ManagerAbility.GetResource("ThrowRock"))
            .ChainFaction(EFaction.PLAYER);
        def_mob1.Move(Vector3i.ONE);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob2 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("PlayerFac2Warrior")
            .ChainAction(Global.ManagerAbility.GetResource("ThrowRock"))
            .ChainFaction(EFaction.PLAYER)
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.WARRIOR) });
        def_mob2.Move(Vector3i.ONE+Vector3i.FORWARD);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob3 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainName("Civilian")
            .ChainAction(Global.ManagerAbility.GetResource("ThrowRock"))
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.CIVILIAN) });
        def_mob3.Move(Vector3i.ONE+Vector3i.LEFT);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob4 = Mob.CreatePrototype(EMobPrototype.HUMAN)
            .ChainJob(new List<Job>() { Job.CreatePrototype(EJob.WIZARD) })
            .ChainAction(Global.ManagerAbility.GetResource("ThrowRock"))
            .ChainName("Neutral wizard");
        def_mob4.Move(Vector3i.ONE+Vector3i.FORWARD+Vector3i.FORWARD);
        //.ChainEquipment(new WeaponSpear());

        encounter.MobPlacement = new();
        encounter.MobPlacement[Vector3i.ONE] =  def_mob1;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.FORWARD] = def_mob2;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.LEFT] = def_mob3;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.LEFT] = def_mob4;
        return encounter;
    }

    public virtual void Finish()
    {

    }

}
