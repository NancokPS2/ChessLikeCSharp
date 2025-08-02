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

	public static void PackSave()
	{
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
	public static void PackLoad(bool user)
	{
		ManagerAbility.ResourceClear(true);
		ManagerAbility.LoadContent(user);

		ManagerMob.ResourceClear(true);
		ManagerMob.LoadContent(user);

		ManagerFaction.ResourceClear(true);
		ManagerFaction.LoadContent(user);

		ManagerItem.ResourceClear(true);
		ManagerItem.LoadContent(user);

		ManagerModel.ResourceClear(true);
		ManagerModel.LoadContent(user);

		ManagerParticle.ResourceClear(true);
		ManagerParticle.LoadContent(user);

		ManagerMaterial.ResourceClear(true);
		ManagerMaterial.LoadContent(user);

		ManagerFont.ResourceClear(true);
		ManagerFont.LoadContent(user);

		ManagerEncounter.ResourceClear(true);
		ManagerEncounter.LoadContent(user);

		ManagerMobTemplate.ResourceClear(true);
		ManagerMobTemplate.LoadContent(user);
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
		resourcePack.LoadContent(false);
		resourcePack.LoadContent(true);
		return resourcePack;
	}
}
