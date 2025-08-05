using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

[GlobalClass]
public partial class SaveFile : Resource
{
	const string CFSECTION_MAIN = "MAIN";
	const string CFKEY_PROFILENAME = "ProfileName";
	const string CFKEY_SAVEDATE = "LastSave";
	const string CFKEY_VERSIONNAME = "GameVersion";

	const string CSFSECTION_STORYFLAGS = "STORY_FLAGS";
	const string CFSECTION_RESOURCE = "RESOURCE";
	const string CFKEY_PLAYERFACTION = "PlayerFaction";

	[Export]
	public string ProfileName = "Unnamed";

	[Export]
	protected Godot.Collections.Dictionary<EStoryFlag, bool> storyFlags
	{
		set => StoryFlags = new(value);
		get => new(StoryFlags);
	}
	private Dictionary<EStoryFlag, bool> StoryFlags = new() { { EStoryFlag.DUMMY, true } };

	[Obsolete("Don't do it like this...")]
	public Faction PlayerFaction;

	protected static string GetSaveBaseFolder() => "user://Save";
	public static bool IsSlotOccupied(string profileName, int slot) => Godot.FileAccess.FileExists($"{GetSaveFile(profileName, slot)}");
	public static string GetSaveFolder(string profileName, int slot) => $"{GetSaveBaseFolder()}/{profileName + slot.ToString()}";
	public static string GetSaveFile(string profileName, int slot) => $"{GetSaveFolder(profileName, slot)}/save.sav";
	public static string GetSaveResourceFolder(string profileName, int slot)
		=> $"{GetSaveFolder(profileName, slot)}/Resources";

	protected void Load(int slot)
		=> Load(ProfileName, slot, false);

/* 		if (!IsSlotOccupied(save.ProfileName, slot))
			throw new Exception($"Cannot load. \nProfile: {save.ProfileName}\nSlot: {slot}"); */
	public static SaveFile Load(string profileName, int slot, bool createIfEmpty)
	{
		SaveFile save;

		bool saveExists = DirAccess.DirExistsAbsolute(GetSaveFolder(profileName, slot));
		
		Global.PackInitialize();
		//If a save already exists, load it.
		if (!saveExists && createIfEmpty)
		{
			save = new();
			save.ProfileName = profileName;
			save.PlayerFaction = Global.ManagerFaction.ResourceGet(EFaction.PLAYER, false);
			save.Save(slot);
			return save;
		}
		else if (!saveExists && !createIfEmpty) throw new Exception($"Save does not exist. \nProfile: {profileName}\nSlot: {slot}");

		//Prepare to make a new one.
		save = new();
		

		//Load resources.
		Global.PackLoad(true);

		//Load ConfigFile
		ConfigFile config = new();
		string configPath = GetSaveFile(profileName, slot);
		config.Load(configPath);

		//Make sure the slot has a save for this profile.
		string loadedProfile = config.GetValue(CFSECTION_MAIN, CFKEY_PROFILENAME).As<string>();
		if (profileName == loadedProfile)
		{
			throw new Exception($"This save file is not from this profile. \nThis profile: {profileName}\nProfile loaded: {loadedProfile}\nPath loaded: {configPath}");
		}

		//Set story flags
		save.StoryFlags = new();
		foreach (var key in config.GetSectionKeys(CSFSECTION_STORYFLAGS))
		{
			save.SetStoryFlag((EStoryFlag)key.ToInt(), config.GetValue(CSFSECTION_STORYFLAGS, key).As<bool>());
		}

		EventBus.LoadAttempted?.Invoke(true);
		return save;
	}

	public static int GetEmptySlot(string profileName)
	{
		int slot = 0;
		while (IsSlotOccupied(profileName, slot))
		{
			slot += 1;
		}
		return slot;
	}

	public bool Save()
	{
		return Save(GetEmptySlot(ProfileName));
	}

	public bool Save(int slot)
	{
		bool success = true;
		if (!Global.PackSave(GetSaveResourceFolder(ProfileName, slot))) success = false;
		ConfigFile config = new();

		//Main stuff
		config.SetValue(CFSECTION_MAIN, CFKEY_PROFILENAME, ProfileName);
		config.SetValue(CFSECTION_MAIN, CFKEY_SAVEDATE, Time.GetUnixTimeFromSystem().ToString());
		config.SetValue(CFSECTION_MAIN, CFKEY_VERSIONNAME, ProjectSettings.GetSettingWithOverride("application/config/version"));

		//Player resources (NO!)
		//config.SetValue(CFSECTION_RESOURCE, CFKEY_PLAYERFACTION, instance.PlayerFaction);

		//Story flags
		foreach (var item in StoryFlags)
		{
			config.SetValue(CSFSECTION_STORYFLAGS, ((int)item.Key).ToString(), item.Value);
		}

		if (config.Save(GetSaveFile(ProfileName, slot)) != Error.Ok) success = false;
		EventBus.SaveAttempted?.Invoke(success);
		return success;
	}

	public static void DeleteSave(string profileName, int slot)
	{
		throw new NotImplementedException();
	}

	public static void DeleteSave(string profileName)
	{

	}

	public void SetStoryFlag(EStoryFlag flag, bool set)
	=> StoryFlags[flag] = set;
			
	/// <summary>
	/// Gets all saves from this profile in the saves folder.
	/// </summary>
	/// <returns>All saves belonging to this profile.</returns>
	public string[] GetAllSaves()
	{
		return DirAccess.GetDirectoriesAt(GetSaveBaseFolder()).Where( x => x.StartsWith(ProfileName)).ToArray();
	}
}
