using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using ChessLike.WorldMap;
using Godot;

//TODO: All direct calls to this class need to emit events instead, then this class can listen to them.
public partial class MsgLog : Node
{
	const string LOG_BASE_FOLDER = "user://LOG";
	Godot.FileAccess LogFile;
	public static MsgLog Instance;
	private CanvasLayer Canvas;
	private static VBoxContainer NodeContainer = new()
	{
		FocusMode = Control.FocusModeEnum.None,
		MouseFilter = Control.MouseFilterEnum.Ignore,
		AnchorRight = 1,
		AnchorBottom = 1,
	};

	public static List<TemporaryLabel> Queue = new();

	public MsgLog()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		for (int i = 0; i < Queue.Count; i++)
		{
			TemporaryLabel? label = Queue[i];
			label.Duration -= delta;

			if (label.Duration <= 0)
			{
				Queue.Remove(label);
				label.AnimateFadeAway(0.5f, true);
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
		DirAccess.MakeDirAbsolute(LOG_BASE_FOLDER);
		string fileName = $"{LOG_BASE_FOLDER}/log{Time.GetDatetimeStringFromDatetimeDict(Time.GetDateDictFromSystem(), false)
			.Replace(":", "")
			.Replace("-", "")}.txt";
		//if (!fileName.IsValidFileName()) throw new Exception($"{fileName} is not a valid log file name.");
		LogFile = Godot.FileAccess.Open(
			fileName,
			Godot.FileAccess.ModeFlags.WriteRead);
		var err = Godot.FileAccess.GetOpenError();
		Canvas = UIManager.GetLayer(UIManager.ELayer.MSG_QUEUE);

		Canvas.AddChild(NodeContainer);
		NodeContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);


		EventBus.MobTurnStarted += OnMobTurnStarted;
		EventBus.CombatStateChanged += OnBattleStateChanged;
		EventBus.MobStateChanged += OnMobStateChanged;
		EventBus.CombatStarted += OnCombatStarted;
		EventBus.MobCommandBroadcasted += AddMessageFromCommand;
		EventBus.SaveAttempted += OnSaveAttempted;
		EventBus.LoadAttempted += OnLoadAttempted;
		EventBus.EncounterLoaded += OnEncounterLoaded;
		EventBus.ActionUsed += OnActionUsed;
		EventBus.ActionPreQueued += OnActionPreQueued;
		EventBus.ActionQueued += OnActionQueued;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		LogFile.Flush();
		LogFile.Close();
	}


	private void AddMessageFromCommand(Dictionary<EInfo, string> dictionary)
	{
		string message = ChessLike.Entity.MobCommand.Command.ParseInfo(dictionary);
		LogGameMsg(message, new() { Duration = message.Length / 6 });
	}

	public void SetAnchors(float left, float top, float right, float bottom)
	{
		NodeContainer.AnchorLeft = left;
		NodeContainer.AnchorTop = top;
		NodeContainer.AnchorRight = right;
		NodeContainer.AnchorBottom = bottom;
	}


	public static void Log(EMessageType type, string text, MessageProperties properties = default)
	{
		string prefix;
		switch (type)
		{
			case EMessageType.INFO:
				prefix = "INFO: ";
				break;

			case EMessageType.ERROR:
				prefix = "ERROR: ";
				GD.PushError(text);
				break;

			case EMessageType.GAMEPLAY:
				prefix = "GAMEPLAY: ";
				AddMessage(text, properties);
				break;

			default: throw new Exception();
		}

		string output = prefix + text + "\n";
		Console.WriteLine(output);
		GD.Print(output);
		Instance.LogFile.StoreString(output);
		Instance.LogFile.Flush();
	}

	public static void LogGameMsg(string text, MessageProperties properties = default)
		=> Log(EMessageType.GAMEPLAY, text, properties);

	public static void LogInfoMsg(string text, MessageProperties properties = default)
		=> Log(EMessageType.INFO, text, properties);

	public static TemporaryLabel AddMessage(string text, MessageProperties properties = default)
	{
		TemporaryLabel new_label = new();
		new_label.Text = text;
		new_label.Duration = properties.Duration;
		NodeContainer.AddChild(new_label);
		Queue.Add(new_label);
		Console.WriteLine("Game: " + text);

		return new_label;
	}

	#region Event Handling
	private void OnBattleStateChanged(ECombatState state)
	{
		LogGameMsg($"State entered {state}");
	}

	private void OnMobTurnStarted(Mob mob)
	{
		LogGameMsg($"{mob.DisplayedName}'s turn started.");
	}

	private void OnMobStateChanged(Mob mob, EMobState state)
	{
		LogGameMsg($"{mob.DisplayedName} entered combat.");
	}

	private void OnCombatStarted()
	{
		LogGameMsg("Combat starts.");
	}

	private void OnSaveAttempted(bool boolean)
	{
		if (boolean)
			LogInfoMsg("Saved successfuly");
		else
			LogInfoMsg("Failed to save");
	}

	private void OnLoadAttempted(bool boolean)
	{
		if (boolean)
			LogInfoMsg("Loaded successfully");
		else
			LogInfoMsg("Failed to load");
	}

	private void OnActionUsed(UsageParameters parameters)
	{
		LogGameMsg($"Used: {parameters.OwnerRef.DisplayedName} by {parameters.ActionRef.Name} on {parameters.MobsTargeted.Count} targets.");
	}

	private void OnActionPreQueued(UsageParameters parameters)
	{
		LogGameMsg($"About to queue action: {parameters.ActionRef.Name} from {parameters.OwnerRef.DisplayedName}.");
	}

	private void OnActionQueued(UsageParameters parameters)
	{
		LogGameMsg($"Queued action: {parameters.ActionRef.Name} from {parameters.OwnerRef.DisplayedName}.");
	}

	private void OnEncounterLoaded(EncounterData obj)
	{
		LogInfoMsg($"Encounter loaded from path '{obj.ResourcePath}' with {obj.MobSpawns.Count} mobs in the following grid: {obj.Grid}");
	}
	#endregion

	public partial class TemporaryLabel : Label
	{
		public double Duration;

		public TemporaryLabel()
		{
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ShrinkEnd;
			FocusMode = Control.FocusModeEnum.None;
			MouseFilter = Control.MouseFilterEnum.Ignore;
			AddThemeStyleboxOverride("normal", new StyleBoxFlat() { BgColor = new(0, 0, 0, 0.1f) });
		}
	}
}
