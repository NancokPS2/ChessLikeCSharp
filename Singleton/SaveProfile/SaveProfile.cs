using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

[GlobalClass]
public partial class SaveProfile : Node
{
	const string CFSECTION_MAIN = "MAIN";
	const string CFKEY_PROFILENAME = "ProfileName";
	const string CFKEY_SAVEDATE = "LastSave";
	const string CFKEY_VERSIONNAME = "GameVersion";

	const string CSFSECTION_STORYFLAGS = "STORY_FLAGS";

	const string CFSECTION_RESOURCE = "RESOURCE";
	const string CFKEY_PLAYERFACTION = "PlayerFaction";
	private static SaveProfile? Instance;

	public string ProfileCurrent = "__UNDEFINED";

	private Dictionary<EStoryFlag, bool> StoryFlags = new(){ {EStoryFlag.DUMMY, true} };

	protected Faction PlayerFaction;

	public SaveProfile() { Instance = this; }


	public static string GetProfilePath() => $"user://Save/{Instance?.ProfileCurrent}/";
	private static string GetProfilePath(string profileOverride) => $"user://Save/{profileOverride}/";

	private static string GetProfileFile(string profileOverride) => $"{GetProfilePath(profileOverride)}/save.sav";

	public static Faction GetPlayerFaction() => Instance?.PlayerFaction ?? throw new Exception();

	public override void _Ready()
	{
		base._Ready();
		EventBus.InputSave += OnInputSave;
		EventBus.InputLoad += OnInputLoad;
	}

	public static void SetProfile(string profile) => (Instance ?? throw new Exception()).ProfileCurrent = profile;

	public static void Load(string profile)
	{
		//Change the profile name
		(Instance ?? throw new Exception()).ProfileCurrent = profile;

		//Make sure the path exists, create it otherwise.
		if (!DirAccess.DirExistsAbsolute(GetProfilePath()))
		{
			Global.PackInitialize();
			Instance.PlayerFaction = Global.ManagerFaction.ResourceGet(EFaction.PLAYER);
			Save();
		}

		//Load resources.
		Global.PackLoad(true);

		//Load ConfigFile
		ConfigFile config = new();
		config.Load(GetProfileFile(profile));

		//Set story flags
		Instance.StoryFlags = new();
		foreach (var key in config.GetSectionKeys(CSFSECTION_STORYFLAGS))
		{
			SetStoryFlag((EStoryFlag)key.ToInt(), config.GetValue(CSFSECTION_STORYFLAGS, key).As<bool>());
		}

		//Get resources (NO!)
		//Instance.PlayerFaction = config.GetValue(CFSECTION_RESOURCE, CFKEY_PLAYERFACTION).As<Faction>();

		//Make sure the profile's config is the same as the folder (don't fuck with the config like that!)
		if (config.GetValue(CFSECTION_MAIN, CFKEY_PROFILENAME).As<string>() != profile)
			throw new Exception();

	}

	public static void Save() => Save((Instance ?? throw new Exception()).ProfileCurrent);

	public static void Save(string profileName)
	{
		SaveProfile instance = Instance ?? throw new Exception();
		Global.PackSave();
		ConfigFile config = new();

		//Main stuff
		config.SetValue(CFSECTION_MAIN, CFKEY_PROFILENAME, profileName);
		config.SetValue(CFSECTION_MAIN, CFKEY_SAVEDATE, Time.GetUnixTimeFromSystem().ToString());
		config.SetValue(CFSECTION_MAIN, CFKEY_VERSIONNAME, ProjectSettings.GetSettingWithOverride("application/config/version"));

		//Player resources (NO!)
		//config.SetValue(CFSECTION_RESOURCE, CFKEY_PLAYERFACTION, instance.PlayerFaction);

		//Story flags
		foreach (var item in instance.StoryFlags)
		{
			config.SetValue(CSFSECTION_STORYFLAGS, ((int)item.Key).ToString(), item.Value);
		}

		config.Save(GetProfileFile(profileName));
	}

	public static void ClearFile(string profile)
	{
		string path = GetProfilePath(profile).TrimSuffix("/");
		Error error = DirAccess.RemoveAbsolute(path);
		if (error != Error.Ok)
			OS.MoveToTrash(path);
	}

	public static void SetStoryFlag(EStoryFlag flag, bool set)
	=> (Instance ?? throw new Exception())
			.StoryFlags[flag] = set;


	#region Event Handling
	private void OnInputLoad(string profile)
	{
		Load(profile);
	}

	private void OnInputSave()
	{
		Save();
	}
	#endregion
}
