using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Storage;
using ChessLike.World.Encounter;
using Godot;

public partial class Global
{
    public static JobResourcePack ManagerJob;
    public static ResourcePack<Ability> ManagerAbility;
    public static MobResourcePack ManagerMob;
    public static FactionResourcePack ManagerFaction;
    public static ItemResourcePack ManagerItem;
    public static SceneResourcePack<Node3D> ManagerModel;
    public static SceneResourcePack<CpuParticles3D> ManagerParticle;
    public static ResourcePack<Material> ManagerMaterial;
    public static ResourcePack<FontFile> ManagerFont;
    public static ResourcePack<EncounterData> ManagerEncounter;
    public static MobTemplateResourcePack ManagerMobTemplate;

    public static void SetupManagers()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerItem = new();
        ManagerModel = new("Model");
        ManagerParticle = new("Particle");
        ManagerMaterial = new("Material");
        ManagerFont = new("Font");
        ManagerEncounter = new();
        ManagerMobTemplate = new();

        PackPrepare(ManagerJob);
        PackPrepare(ManagerAbility);
        PackPrepare(ManagerMob);
        PackPrepare(ManagerFaction);
        PackPrepare(ManagerItem);
        PackPrepare(ManagerModel);
        PackPrepare(ManagerParticle);
        PackPrepare(ManagerMaterial);
        PackPrepare(ManagerFont);
        PackPrepare(ManagerEncounter);
        PackPrepare(ManagerMobTemplate);
    }

    public static void PackSave()
    {
        ManagerJob.SavePersistent();
        ManagerAbility.SavePersistent();
        ManagerMob.SavePersistent();
        ManagerFaction.SavePersistent();
        ManagerItem.SavePersistent();
        ManagerModel.SavePersistent();
        ManagerParticle.SavePersistent();
        ManagerMaterial.SavePersistent();
        ManagerFont.SavePersistent();
        ManagerEncounter.SavePersistent();
        ManagerMobTemplate.SavePersistent();
    }

    private static ResourcePack<T> PackPrepare<T>(ResourcePack<T> resourcePack) where T : Resource, new()
    {
        resourcePack.CreateDefault();
        resourcePack.LoadContent(false, true);
        resourcePack.LoadContent(true, true);
        return resourcePack;
    }
}
