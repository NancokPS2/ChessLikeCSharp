using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

[GlobalClass]
public partial class SaveManager : Node
{
	const string CFSECTION_MAIN = "MAIN";
	const string CFKEY_PROFILENAME = "ProfileName";
	const string CFKEY_SAVEDATE = "LastSave";
	const string CFKEY_VERSIONNAME = "GameVersion";

	const string CSFSECTION_STORYFLAGS = "STORY_FLAGS";
	const string CFSECTION_RESOURCE = "RESOURCE";
	const string CFKEY_PLAYERFACTION = "PlayerFaction";
	private const string SAVE_IDENTIFIER_DEFAULT = "__UNDEFINED";

	private static SaveManager? Instance;

	public SaveManager() { Instance = this; }

	protected static SaveFile? CurrentSave;
	protected static int SaveSlot;

	private static string GetSaveFolder(string saveIdentifierOverride) => $"user://Save/{saveIdentifierOverride}/";
	public static string GetSaveFile(string saveIdentifierOverride) => $"{GetSaveFolder(saveIdentifierOverride)}/save.sav";


	public override void _Ready()
	{
		base._Ready();
		EventBus.InputSave += OnInputSave;
		EventBus.InputLoad += OnInputLoad;
		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
	}

	protected static SaveFile GetNewSave(string profileName, int slot)
	{
		SaveFile newSave = new SaveFile(profileName);
		SetCurrentSave(newSave, slot);
		Save(true);
		return newSave;
	}

	protected static SaveFile GetLoadedSave(string profileName, int slot)
	{
		SaveFile save = SaveFile.Load(profileName, slot, false);
		return save;
	}

	public static SaveFile? GetCurrentSave() => CurrentSave;
	public static int GetCurrentSlot() => SaveSlot;

	public static List<(SaveFile, int)> GetAllSaves()
	{
		List<(SaveFile, int)> output = new();
		foreach (var item in from dir in DirAccess.GetDirectoriesAt(SaveFile.GetSaveBaseFolderPath()) select dir.Split("_"))
		{
			if (item.Count() != 2)
				throw new Exception($"Invalid save directory found {item.ToStringList()}");

			if (!item[1].IsValidInt())
				throw new Exception($"Not a valid integer slot on the directory {item[0]} {item[1]}.");

			if (!SaveFile.IsSlotOccupied(item[0], item[1].ToInt()))
				throw new Exception($"Could not determine that this directory is a proper occupied save slot {item[0]} {item[1]}");

			string profile = item[0];
			int slot = item[1].ToInt();

			output.Add((SaveFile.Load(profile, slot, false), slot));
		}
		return output;
	}

	private static void SetCurrentSave(SaveFile file, int slot)
	{
		CurrentSave = file;
		SaveSlot = slot;
	}

	public static void NewSave(string profileName, int slot)
	{
		SetCurrentSave(GetNewSave(profileName, slot), slot);
	}

	public static void LoadSave(string profileName, int slot)
	{
		if (!SaveFile.IsSlotOccupied(profileName, slot))
			throw new Exception($"Cannot load {profileName}, slot {slot}. It does not exist.");

		SetCurrentSave(GetLoadedSave(profileName, slot), slot);

		EventBus.LoadAttempted?.Invoke(true);
	}

	public static void ReloadSave()
	{
		if (CurrentSave is null)
			throw new Exception($"There is no SaveFile set.");
		LoadSave(CurrentSave.ProfileName, SaveSlot);
	}

	public static bool SaveExists(string profile, int slot)
		=> SaveFile.IsSlotOccupied(profile, slot);

	public static void DeleteSave(string profileName, int slot)
	{
		SaveFile.DeleteSave(profileName, slot);
	}

	public static bool Save(bool overwrite)
	{
		if (CurrentSave is null) throw new Exception();

		if (!overwrite && SaveFile.IsSlotOccupied(CurrentSave.ProfileName, SaveSlot))
			throw new Exception("Cannot overwrite save file.");

		bool success = CurrentSave.Save(SaveSlot);
		EventBus.SaveAttempted?.Invoke(success);
		return success;
	}

	public static bool IsStoryFlagSet(EStoryFlag flag)
	{
		return CurrentSave?.GetStoryFlag(flag) ?? throw new Exception();
	}

	#region Event Handling
	private void OnInputLoad(string profile, int slot)
	{
		UIManager.ChangeToUI(EUIScene.SAVE_SELECT);
		if (!SaveExists(profile, slot)) throw new Exception($"Tried to load not existant save {profile} {slot}");
		LoadSave(profile, slot);
	}

	private void OnInputSave()
	{
		Save(true);
	}
	
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.SAVE)
			OnInputSave();
	}
	#endregion
}
