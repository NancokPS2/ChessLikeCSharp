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
	private const string SAVE_IDENTIFIER_DEFAULT = "__UNDEFINED";
	private static SaveProfile? Instance;

	protected string SaveIdentifier = SAVE_IDENTIFIER_DEFAULT;
	protected string ProfileName = "Unnamed";

	private Dictionary<EStoryFlag, bool> StoryFlags = new() { { EStoryFlag.DUMMY, true } };

	protected Faction PlayerFaction;

	public SaveProfile() { Instance = this; }


	public static string GetProfilePath() => $"user://Save/{Instance?.SaveIdentifier}/";
	private static string GetProfilePath(string saveIdentifierOverride) => $"user://Save/{saveIdentifierOverride}/";

	private static string GetProfileFile(string saveIdentifierOverride) => $"{GetProfilePath(saveIdentifierOverride)}/save.sav";

	public static Faction GetPlayerFaction() => Instance?.PlayerFaction ?? throw new Exception();

	public override void _Ready()
	{
		base._Ready();
		EventBus.InputSave += OnInputSave;
		EventBus.InputLoad += OnInputLoad;
		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
	}

	public static void SetProfileName(string profile)
		=> (Instance ?? throw new Exception()).ProfileName = profile;

	public static void SetSaveIdentifier(string saveIdentifier)
		=> (Instance ?? throw new Exception()).SaveIdentifier = saveIdentifier;

	public static void Load(string saveIdentifier)
	{
		//Change the identifier
		SetSaveIdentifier(saveIdentifier);

		//Make sure the path exists, create it otherwise.
		if (!DirAccess.DirExistsAbsolute(GetProfilePath()))
		{
			Global.PackInitialize();
			Instance.PlayerFaction = Global.ManagerFaction.ResourceGet(EFaction.PLAYER, false);
			Save(saveIdentifier);
		}

		//Load resources.
		Global.PackLoad(true);

		//Load ConfigFile
		ConfigFile config = new();
		config.Load(GetProfileFile(saveIdentifier));

		//Load profile name.
		SetProfileName(config.GetValue(CFSECTION_MAIN, CFKEY_PROFILENAME).As<string>());

		//Set story flags
		Instance.StoryFlags = new();
		foreach (var key in config.GetSectionKeys(CSFSECTION_STORYFLAGS))
		{
			SetStoryFlag((EStoryFlag)key.ToInt(), config.GetValue(CSFSECTION_STORYFLAGS, key).As<bool>());
		}

		//Get resources (NO!)
		//Instance.PlayerFaction = config.GetValue(CFSECTION_RESOURCE, CFKEY_PLAYERFACTION).As<Faction>();
		EventBus.LoadAttempted?.Invoke(true);
	}

	public static bool Save() => Save((Instance ?? throw new Exception()).SaveIdentifier);

	public static bool Save(string saveIdentifier)
	{
		SaveProfile instance = Instance ?? throw new Exception();
		bool success = true;
		if (!Global.PackSave()) success = false;
		ConfigFile config = new();

		//Main stuff
		config.SetValue(CFSECTION_MAIN, CFKEY_PROFILENAME, instance.ProfileName);
		config.SetValue(CFSECTION_MAIN, CFKEY_SAVEDATE, Time.GetUnixTimeFromSystem().ToString());
		config.SetValue(CFSECTION_MAIN, CFKEY_VERSIONNAME, ProjectSettings.GetSettingWithOverride("application/config/version"));

		//Player resources (NO!)
		//config.SetValue(CFSECTION_RESOURCE, CFKEY_PLAYERFACTION, instance.PlayerFaction);

		//Story flags
		foreach (var item in instance.StoryFlags)
		{
			config.SetValue(CSFSECTION_STORYFLAGS, ((int)item.Key).ToString(), item.Value);
		}

		if (config.Save(GetProfileFile(saveIdentifier)) != Error.Ok) success = false;
		EventBus.SaveAttempted?.Invoke(success);
		return success;
	}

	public static void ClearFile(string profile)
	{
		string path = GetProfilePath(profile).TrimSuffix("/");
		SetSaveIdentifier(SAVE_IDENTIFIER_DEFAULT);
		Error error = DirAccess.RemoveAbsolute(path);
		if (error != Error.Ok)
			OS.MoveToTrash(path);
	}

	public static void SetStoryFlag(EStoryFlag flag, bool set)
	=> (Instance ?? throw new Exception())
			.StoryFlags[flag] = set;

	public static string[] GetAllSaves()
	{
		return DirAccess.GetDirectoriesAt("user://Save");
	}


	#region Event Handling
	private void OnInputLoad(string profile)
	{
		Load(profile);
	}

	private void OnInputSave()
	{
		Save();
	}
	
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.SAVE)
			OnInputSave();
	}
	#endregion
}
