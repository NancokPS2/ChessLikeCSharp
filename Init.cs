using ChessLike.Singleton;
using Godot;
using System;

public partial class Init : Node
{
	public const string ARGUMENT_PROFILE = "profile";
	public override void _Ready()
	{
		base._Ready();

		ProcessCmdArguments();

		QueueFree();
	}

	protected void ProcessCmdArguments()
	{
		string[] arguments = OS.GetCmdlineUserArgs();

		//TEMP
		arguments = ["++profile=nuevo"];

		string argumentProfile = "";

		MsgLog.LogInfoMsg(arguments.ToString());

		foreach (var item in arguments)
		{
			string[] text = item.Replace("--", "").Replace("++", "").Split("=");
			if (text.Length != 2)
				throw new Exception($"Invalid argument {text}");

			switch (text[0])
			{
				case ARGUMENT_PROFILE:
					argumentProfile = text[1];
					break;

				default:
					break;
			}
		}

		if (argumentProfile != "")
			SaveManager.LoadSave(argumentProfile, 0);
		else
			SceneChanger.ChangeToMainMenu();
	}

	protected void AutoStart(string profileName)
	{
		SaveManager.LoadSave(profileName, 0);
	}
}
