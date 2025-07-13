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

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerMob = new();
        ManagerFaction = new() { AutoPoolLoaded = true };
        ManagerItem = new();
        ManagerModel = new("Model");
        ManagerParticle = new("Particle");
        ManagerMaterial = new("Material");
        ManagerFont = new("Font");
        ManagerEncounter = new();
        ManagerMobTemplate = new();

        PreparePack(ManagerJob);
        PreparePack(ManagerAbility);
        PreparePack(ManagerMob);
        PreparePack(ManagerFaction);
        PreparePack(ManagerItem);
        PreparePack(ManagerModel);
        PreparePack(ManagerParticle);
        PreparePack(ManagerMaterial);
        PreparePack(ManagerFont);
        PreparePack(ManagerEncounter);
        PreparePack(ManagerMobTemplate);
    }

    private static ResourcePack<T> PreparePack<T>(ResourcePack<T> resourcePack) where T : Resource, new()
    {
        resourcePack.CreateDefault();
        resourcePack.LoadAllInFolder();
        return resourcePack;
    }
}
