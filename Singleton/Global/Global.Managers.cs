using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;
using ChessLike.World.Encounter;
using Godot;

public partial class Global
{
    public static JobResourcePack ManagerJob;
    public static ResourcePack<Ability> ManagerAbility;
    public static MobResourcePack ManagerMob;
    public static FactionResourcePack ManagerFaction;
    public static ResourcePack<Inventory> ManagerInventory;
    public static ItemResourcePack ManagerItem;
    public static SceneResourcePack<Node3D> ManagerModel;
    public static SceneResourcePack<CpuParticles3D> ManagerParticle;
    public static ResourcePack<Material> ManagerMaterial;
    public static ResourcePack<FontFile> ManagerFont;
    public static ResourcePack<EncounterData> ManagerEncounter;

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerMob = new();
        ManagerFaction = new(){AutoPoolLoaded = true};
        ManagerInventory = new();
        ManagerItem = new();
        ManagerModel = new("Model");
        ManagerParticle = new("Particle");
        ManagerMaterial = new("Material");
        ManagerFont = new("Font");
        ManagerEncounter = new();

        ManagerJob.CreateDefault();
        ManagerAbility.CreateDefault();
        ManagerMob.CreateDefault();
        ManagerFaction.CreateDefault();
        ManagerInventory.CreateDefault();
        ManagerItem.CreateDefault();
        ManagerModel.CreateDefault();
        ManagerParticle.CreateDefault();
        ManagerMaterial.CreateDefault();
        ManagerFont.CreateDefault();
        ManagerEncounter.CreateDefault();

        ManagerJob.LoadAllInFolder();
        ManagerAbility.LoadAllInFolder();
        ManagerMob.LoadAllInFolder();
        ManagerFaction.LoadAllInFolder();
        ManagerInventory.LoadAllInFolder();
        ManagerItem.LoadAllInFolder();
        ManagerModel.LoadAllInFolder();
        ManagerParticle.LoadAllInFolder();
        ManagerMaterial.LoadAllInFolder();
        ManagerFont.LoadAllInFolder();
        ManagerEncounter.LoadAllInFolder();

        Job testJob = new Job();
        testJob.SetMeta("TEST", 94);
        ResourceSaver.Save(testJob, "user://temp.tres");
    }
}
