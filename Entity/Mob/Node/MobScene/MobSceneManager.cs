using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using LightInject;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobSceneManager : Node3D, ISingleton<MobSceneManager>
{
	protected const string META_ALLOWED_IN_STORAGE = "META_MobSceneManager_IS_ALLOWED_IN_STORAGE";
	public const uint STORAGE_MAX_INSTANCES = 200;
	protected readonly static Godot.Vector3 STORAGE_ROOT_POSITION = new Godot.Vector3(0, float.MaxValue * 0.6f, 0);
	protected readonly static float STORAGE_SPACING = 4;

	protected static (Vector3i, Godot.Vector3)[] StoragePositions;
	protected static Camera3D? StorageLastCamera;
	protected static Mob[] StorageShownMobs = [];

	public static MobSceneManager? Instance { get => IsInstanceValid(instance) ? instance : null; set => instance = value; }
	private static MobSceneManager instance;

	[Export]
	protected float CursorSpeed = 15;
	[Export]
	protected Camera3D StorageCamera = null!;
	protected Node3D NodeSelectionCursor = GD.Load<PackedScene>("uid://4cikkiw1mfd").Instantiate<Node3D>();
	protected Node3D NodeHoveringCursor = GD.Load<PackedScene>("uid://cu1nlfq5x61rn").Instantiate<Node3D>();
	protected Node3D NodeCurrentlySelected = GD.Load<PackedScene>("uid://dsomybklmhgee").Instantiate<Node3D>();

	protected UniqueList<MobScene> InstancedMobs = new();

	protected Mob? SelectedMob;
	protected MobScene? SelectedMobScene;


	public MobSceneManager()
	{
		Vector3i gridPos = Vector3i.ZERO;
		
		for (int i = 0; i < STORAGE_MAX_INSTANCES; i++)
		{
			Godot.Vector3 realPos = gridPos.ToGVector3() * STORAGE_SPACING;
			StoragePositions[i] = (gridPos, realPos);
			gridPos += Vector3i.FORWARD;
		}
	}

	public override void _Ready()
	{
		base._Ready();
		Instance = this;

		EventBus.MobStateChanged += OnMobStateChanged;
		EventBus.CellPositionSelected += OnCellSelected;
		EventBus.CellPositionHovered += OnCellHovered;
		EventBus.MobTurnStarted += OnMobTurnStarted;
		EventBus.MobSelected += OnMobSelected;

		ConnectDialogue();

		AddChild(NodeSelectionCursor);
		AddChild(NodeHoveringCursor);
		AddChild(NodeCurrentlySelected);
	}

	protected override void Dispose(bool disposing)
	{
		EventBus.MobStateChanged -= OnMobStateChanged;
		EventBus.CellPositionSelected -= OnCellSelected;
		EventBus.CellPositionHovered -= OnCellHovered;
		EventBus.MobTurnStarted -= OnMobTurnStarted;
		Instance = null;
		base.Dispose(disposing);
	}


	public override void _Process(double delta)
	{
		base._Process(delta);

		//Make the selected mob scene scene match the selected mob
		SelectedMobScene = SelectedMob is null ? null : GetInstance(SelectedMob);

		//Move the cursor to the target scene position
		Godot.Vector3 target = SelectedMobScene?.GlobalPosition ?? Godot.Vector3.Zero;
		NodeSelectionCursor.GlobalPosition = NodeSelectionCursor.GlobalPosition.MoveToward(
			target, (float)(CursorSpeed * delta)
			);

		//Make it visible if there is a mob scene.
		NodeSelectionCursor.Visible = SelectedMobScene is not null;

		//Move the currently selected cursor to the mob's position

		Godot.Vector3 currentSelectedTarget;
		if (SelectedMobScene is not null)
		{
			currentSelectedTarget = CombatScene.GetGridNode().MapToGlobal(SelectedMobScene.MobUsing.GetPosition());
			NodeCurrentlySelected.Show();
			NodeCurrentlySelected.GlobalPosition = currentSelectedTarget;
		}
		else
			NodeCurrentlySelected.Hide();

		DialogueProcess(delta);
	}

	public static MobScene? GetMobScene(Mob mob)
	{
		return Instance?.GetInstance(mob);
	}

	private bool HasInstance(Mob mob)
		=> InstancedMobs.Any(x => x.MobUsing == mob);


	private MobScene? GetInstance(Mob mob)
	{
		List<MobScene> instancesFound = InstancedMobs.FindAll(x => x.MobUsing == mob);
		if (instancesFound.Count > 1)
		{
			throw new Exception($"Only one instance should exist. Found {instancesFound.Count}");
		}
		//If there is none, return null.
		else if (instancesFound.Count == 0)
		{
			return null;
		}
		//If there is one, return that.
		else
		{
			return instancesFound[0];
		}
	}

	protected MobScene CreateInstance(Mob mob)
	{
		MobScene newInstance = Readonly.Scenes.SCENE_MOB;
		newInstance.MobUsing = mob;
		return newInstance;
	}

	private void AddInstance(Mob mob)
	{
		MobScene instance = GetInstance(mob) ?? CreateInstance(mob);

		if (HasInstance(mob)) return;

		InstancedMobs.Add(instance, false);
		AddChild(instance);
		instance.MovementResetPosition();
	}

	public void FreeInstance(Mob mob)
	{
		if (!HasInstance(mob)) return;
		MobScene instance = GetInstance(mob) ?? throw new Exception("Inconsistency between HasInstance() and GetInstance()");

		instance.QueueFree();
		InstancedMobs.Remove(instance);
	}

	protected MobScene? GetInstanceByPosition(Vector3i cellPos)
	{
		foreach (var instance in InstancedMobs)
		{
			if (instance.MobUsing.GetPosition() == cellPos)
			{
				return instance;
			}
		}
		return null;
	}

protected void SelectMob(MobScene scene)
		=> SelectMob(scene.MobUsing);

	protected void SelectMob(Mob mob)
	{
		MobScene? scene = GetInstanceByPosition(mob.GetPosition());
		if (scene is null) return;

		//Unnecesary, OnMobSelected() handles this already.
		//SelectedMobScene = GetInstance(mob);

		EventBus.MobSelected?.Invoke(scene.MobUsing);
	}

	protected void HoverMob(MobScene scene)
		=> HoverMob(scene.MobUsing);
	protected void HoverMob(Mob mob)
	{
		MobScene? scene = GetInstanceByPosition(mob.GetPosition());
		if (scene is null) return;

		NodeHoveringCursor.GlobalPosition = scene.MarkerOverhead.GlobalPosition;
		NodeHoveringCursor.Show();
		EventBus.MobHovered?.Invoke(scene.MobUsing);
	}

	private void ThrowOnMissingInstance(Mob mob)
	{
		if (!HasInstance(mob)) throw new Exception();
	}

	#region Storage
	protected void StorageSetAllowed(Mob mob, bool allowed)
	{
		MobScene? instance = GetInstance(mob) ?? throw new Exception($"This Mob ({mob}) does not have a MobScene yet.");
		instance.SetMeta(META_ALLOWED_IN_STORAGE, allowed);
	}

	protected bool StorageIsMobAllowed(Mob mob)
	{
		return mob.GetMeta(META_ALLOWED_IN_STORAGE, false).As<bool>();
	}

	protected Mob[] GetStorageMobs(EMobState requiredState)
		=> Mob.GetInstancesInState(requiredState).Where(x => StorageIsMobAllowed(x)).ToArray();

	public void StorageShow(EMobState state, EFaction[] factions)
	{
		StorageHide();

		//Update the camera so it can return there.
		Camera3D currentCamera = GetViewport().GetCamera3D();
		if (StorageCamera != currentCamera)
		{
			StorageLastCamera = currentCamera;
		}
		StorageCamera.MakeCurrent();

		//Prepare to decide which mobs will go in the storage
		StorageShownMobs = GetStorageMobs(state);

		StorageSpreadInstances(StorageShownMobs);
	}

	public void StorageHide()
	{
		foreach (var item in StorageShownMobs)
		{
			if (GetInstance(item) is MobScene scene)
				scene.IgnoreMobPosition = false;
		}
		StorageLastCamera?.MakeCurrent();
	}

	protected void StorageSpreadInstances(Mob[] mobs)
	{
		//Godot.Vector3 spacing = Godot.Vector3.Forward * STORAGE_SPACING;
		//Godot.Vector3 spacingNewLine = Godot.Vector3.Right * STORAGE_SPACING;
		//Vector3i vec3i = Vector3i.ZERO;
		int count = 0;
		foreach (var item in mobs)
		{
			MobScene instance = GetInstance(item) ?? CreateInstance(item);
			Godot.Vector3 designatedPos = StoragePositions[count].Item2;
			instance.IgnoreMobPosition = true;
			instance.GlobalPosition = designatedPos;
			count++;
		}
	}
	#endregion

	#region Event Handling
	private void OnMobSelected(Mob obj)
	{
		SelectedMob = obj;
	}

	private void OnMobStateChanged(Mob mob, EMobState state)
	{
		if (state == EMobState.COMBAT)
		{
			//Has an instance, skip and keep using that.
			if (HasInstance(mob)) return;
			//Add an instance for this mob.
			AddInstance(mob);
		}
		else if (state == EMobState.BENCHED)
		{
			//Send it to storage.
			AddInstance(mob);
		}
		else
		{
			//Does not have an instance already, skip.
			if (!HasInstance(mob)) return;
			//Has an instance, remove it.
			else FreeInstance(mob);
		}
	}

	private void OnCellSelected(Vector3i cellPos)
	{
		//Do not select anything if in targeting state.
		if (CombatScene.GetState() == ECombatState.TARGETING) return;

		//Try to find an instance.
		MobScene? scene = GetInstanceByPosition(cellPos);
		if (scene is null) return;

		SelectMob(scene);
	}

	private void OnCellHovered(Vector3i cellPos)
	{
		//Try to find an instance.
		MobScene? scene = GetInstanceByPosition(cellPos);
		if (scene is not null)
		{
			HoverMob(scene);
		}
		else
		{
			NodeHoveringCursor.Hide();
		}

	}

	private void OnMobTurnStarted(Mob mob)
	{
		ThrowOnMissingInstance(mob);
		MobScene instance = GetInstance(mob) ?? throw new Exception();

		//Turn Start
		mob.TurnActive = true;
		mob.Stats.RefillValues([EValueName.ACTION, EValueName.SUB_ACTION, EValueName.REACTION, EValueName.MOVE]);
		instance.AnimatePopupText("READY");
		instance.AnimateAddEffect(MobScene.EMobSceneEffect.TURN_ACTIVE, true);

		SelectMob(mob);
	}

    #endregion
}
