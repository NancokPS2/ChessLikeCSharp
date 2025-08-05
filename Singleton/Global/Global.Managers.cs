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


		PackInitialize();
	}

	public static bool PackSave(string baseFolder)
	{
		List<bool> success = new(){
			ManagerAbility.SavePersistent(baseFolder),
			ManagerMob.SavePersistent(baseFolder),
			ManagerFaction.SavePersistent(baseFolder),
			ManagerItem.SavePersistent(baseFolder),
			ManagerModel.SavePersistent(baseFolder),
			ManagerParticle.SavePersistent(baseFolder),
			ManagerMaterial.SavePersistent(baseFolder),
			ManagerFont.SavePersistent(baseFolder),
			ManagerEncounter.SavePersistent(baseFolder),
			ManagerMobTemplate.SavePersistent(baseFolder),
		};
		return success.All(x => x == true);
	}
	public static void PackLoad(bool user)
	{
		ManagerAbility.ResourceClear(true);
		ManagerAbility.LoadContent();

		ManagerMob.ResourceClear(true);
		ManagerMob.LoadContent();

		ManagerFaction.ResourceClear(true);
		ManagerFaction.LoadContent();

		ManagerItem.ResourceClear(true);
		ManagerItem.LoadContent();

		ManagerModel.ResourceClear(true);
		ManagerModel.LoadContent();

		ManagerParticle.ResourceClear(true);
		ManagerParticle.LoadContent();

		ManagerMaterial.ResourceClear(true);
		ManagerMaterial.LoadContent();

		ManagerFont.ResourceClear(true);
		ManagerFont.LoadContent();

		ManagerEncounter.ResourceClear(true);
		ManagerEncounter.LoadContent();

		ManagerMobTemplate.ResourceClear(true);
		ManagerMobTemplate.LoadContent();
	}

	public static void PackInitialize()
	{
		PackInitialize(ManagerAbility);
		PackInitialize(ManagerMob);
		PackInitialize(ManagerFaction);
		PackInitialize(ManagerItem);
		PackInitialize(ManagerModel);
		PackInitialize(ManagerParticle);
		PackInitialize(ManagerMaterial);
		PackInitialize(ManagerFont);
		PackInitialize(ManagerEncounter);
		PackInitialize(ManagerMobTemplate);
	}

	public static ResourcePack<T> PackInitialize<T>(ResourcePack<T> resourcePack) where T : Resource, new()
	{
		resourcePack.CreateDefault();
		resourcePack.ResourceClear(false);
		resourcePack.ResourceClear(true);
		resourcePack.LoadContent();
		return resourcePack;
	}
}
