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

        Mob def_mob1 = new Mob().ChainName("PlayerFac").ChainFaction(EFaction.PLAYER);
        def_mob1.Move(Vector3i.ONE);
        Global.ManagerMobTemplate.GetResource( EPackIDMobTemplate.RaceHuman ).ApplyTemplate(def_mob1);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob2 = new Mob().ChainName("HumanTemplate").ChainFaction(EFaction.PLAYER);
        def_mob2.Move(Vector3i.ONE+Vector3i.FORWARD);
        Global.ManagerMobTemplate.GetResource( EPackIDMobTemplate.RaceHuman ).ApplyTemplate(def_mob2);
        
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob3 = new Mob().ChainName("Bandit").ChainFaction(EFaction.NEUTRAL);
        def_mob3.Move(Vector3i.ONE+Vector3i.LEFT);
        Global.ManagerMobTemplate.GetResource( EPackIDMobTemplate.Extra_Bandit ).ApplyTemplate(def_mob3);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

        Mob def_mob4 = new Mob().ChainName("Neutral Wizard").ChainFaction(EFaction.NEUTRAL);
        def_mob4.Move(Vector3i.ONE+Vector3i.FORWARD+Vector3i.FORWARD);
        Global.ManagerMobTemplate.GetResource( EPackIDMobTemplate.RaceHuman ).ApplyTemplate(def_mob4);

        //Test persistence
        Mob persistentMob = Global.ManagerMob.ResourceGet("Default");
        persistentMob.MakePersistent();
        persistentMob.DisplayedName = "Persistent MC";
        Global.ManagerMob.PooledAdd(persistentMob);
        Global.PackSave();
        Global.ManagerMob.LoadContent(true, true);
        Mob def_mob5 = Global.ManagerMob.ResourceGet("Persistent MC", true);

        encounter.MobPlacement = new();
        encounter.MobPlacement[Vector3i.ONE] =  def_mob1;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.FORWARD] = def_mob2;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.LEFT] = def_mob3;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.FORWARD*2] = def_mob4;
        encounter.MobPlacement[Vector3i.ONE+Vector3i.FORWARD*4] = def_mob5;
        return encounter;
    }

    public virtual void Finish()
    {

    }

}
