using ChessLike.Entity;
using ChessLike.Storage;
using ChessLike.World;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class EncounterData : Resource
{
    [Export]
    public Grid Grid = new();

    public List<MobSpawn> MobSpawns = new();
    [Export]
    private Godot.Collections.Array<MobSpawn> mobSpawns
    {
        set
        {
			MobSpawns = new(value);
        }
        get
        {
            return new(MobSpawns);
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
        bool no_hostiles_remaining = !Mob.GetInstancesInCombat()
            .Any(x => x.GetFaction()
			.IsEnemy(EFaction.PLAYER));

        bool turn_limit_reached = RoundLimit > 0 && RoundCount >= RoundLimit;

        return no_hostiles_remaining || turn_limit_reached;
    }

    public static EncounterData GetDefault()
    {
        EncounterData encounter = new();
        encounter.Grid = GridTerrainGenerator.GenerateFlat(new(6));
		encounter.Grid.FillCellWhere(
			GridCell.Preset.Spawnpoint,
			pos =>
				//In the X border.
				pos.X == encounter.Grid.Boundary.X - 1
				//If it is in empty air.
				&& encounter.Grid.GetCell(pos) == GridCell.Preset.Air
				//If it has a floor below.
				&& encounter.Grid.IsFlagInPosition(pos + Vector3i.DOWN, ECellFlag.SOLID)
			);

		MobTemplateBase templateBase =
			Global.ManagerMobTemplate.GetResource<MobTemplateBase>(EPackIDMobTemplate.Default);
        MobTemplateRace templateHuman =
            Global.ManagerMobTemplate.GetResource<MobTemplateRace>(EPackIDMobTemplate.RaceHuman);
        MobTemplateJob templateWizard =
            Global.ManagerMobTemplate.GetResource<MobTemplateJob>(EPackIDMobTemplate.JobWizard);

        MobSpawn mobSpawn1 = new();
		mobSpawn1.TemplatesBase.Add(templateBase);
        mobSpawn1.TemplatesRace.Add(templateHuman);
		//def_mob1.TemplateUpdate(true, true);

        MobSpawn mobSpawn2 = new();
		mobSpawn2.TemplatesBase.Add(templateBase);
        mobSpawn2.TemplatesRace.Add(templateHuman);

        MobSpawn mobSpawn3 = new();
		mobSpawn3.TemplatesBase.Add(templateBase);
        mobSpawn3.TemplatesRace.Add(templateHuman);
        mobSpawn3.TemplatesJob.Add(templateWizard);
        //.ChainEquipment(Global.ManagerItem.GetFromEnum(ChessLike.Storage.EItem.SWORD));

		//Test persistence
		SaveManager.NewSave("EncounterLoadingTest", 0);
        Mob persistentMob = Global.ManagerMob.ResourceGet("Default");
        persistentMob.MakePersistent();
        persistentMob.DisplayedName = "Persistent MC";
        Global.ManagerMob.ResourceAddPersistent(persistentMob.DisplayedName, persistentMob);
		SaveManager.Save(true);
		SaveManager.ReloadSave();
        Mob def_mob5 = Global.ManagerMob.ResourceGet("Persistent MC", true);


        encounter.MobSpawns = new(){
			mobSpawn1,
			mobSpawn2,
			mobSpawn3,
			new(){Mob = def_mob5}
		};
        return encounter;
    }

    public virtual void Finish()
    {

    }

}
