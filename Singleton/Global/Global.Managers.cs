using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Storage;
using ChessLike.WorldMap;
using ChessLike.WorldMap;
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

	public static List<IResourcePack> GetAllResourcePacks()
	{
		//IMPORTANT: The order here can help with failure to load resources. Apparently, the more dependencies, the further up it should go.
		return new()
		{
			ManagerEncounter,
			ManagerAbility,
			ManagerMob,
			ManagerFaction,
			ManagerItem,
			ManagerModel,
			ManagerParticle,
			ManagerMaterial,
			ManagerFont,
			ManagerMobTemplate,
		};
	}

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
		List<bool> success = new();
		foreach (var item in GetAllResourcePacks())
		{
			success.Add(item.SavePersistent(baseFolder));
		}
		return success.All(x => x == true);
	}
	public static void PackLoad(string userFolder)
	{
		foreach (var item in GetAllResourcePacks())
		{
			item.ResourceClear(false);
			item.ResourceClear(true);
			item.LoadContent();
			item.LoadContentPersistent(userFolder);
		}
	}

	public static void PackInitialize()
	{
		foreach (var item in GetAllResourcePacks())
		{
			item.CreateDefault();
			item.ResourceClear(false);
			item.ResourceClear(true);
			item.LoadContent();
		}
	}
}
