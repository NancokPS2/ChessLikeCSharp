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

	public static SaveFile? SaveCurrent;
	public static int SaveSlotCurrent;

	private static string GetSaveFolder(string saveIdentifierOverride) => $"user://Save/{saveIdentifierOverride}/";
	public static string GetSaveFile(string saveIdentifierOverride) => $"{GetSaveFolder(saveIdentifierOverride)}/save.sav";


	public override void _Ready()
	{
		base._Ready();
		EventBus.InputSave += OnInputSave;
		EventBus.InputLoad += OnInputLoad;
		EventBus.InputPauseOptionSelected += OnInputPauseOptionSelected;
	}

	public static SaveFile NewSave(string profileName, int slot)
	{
		SaveFile.DeleteSave(profileName, slot);
		SaveFile newSave = SaveFile.Load(profileName, slot, true);
		SaveCurrent = newSave;
		return newSave;
	}

	public static SaveFile Load(string profileName, int slot)
	{
		SaveFile save = SaveFile.Load(profileName, slot, false);
		return save;
	}

	public static bool Save(SaveFile file, int slot, bool overwrite)
	{
		if (!overwrite && SaveFile.IsSlotOccupied(file.ProfileName, slot))
			throw new Exception("Cannot overwrite save file.");

		return file.Save(slot);
	}
	#region Event Handling
	private void OnInputLoad(string profile)
	{
	}

	private void OnInputSave()
	{
	}
	
	private void OnInputPauseOptionSelected(EPauseOption obj)
	{
		if (obj == EPauseOption.SAVE)
			OnInputSave();
	}
	#endregion
}
