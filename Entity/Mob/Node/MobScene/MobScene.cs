using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Extension;
using ChessLike.World;
using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class MobScene : Node3D
{
	public Mob MobUsing;

	protected PackedScene FloatingIconScene = GD.Load<PackedScene>("uid://bmm3h2202bdkq");

	public StatusEffectIcon StatusEffectIcon { get => statusEffectIcon ?? throw new Exception(); set => statusEffectIcon = value; }
	[Export]
	private StatusEffectIcon? statusEffectIcon;

	public Node3D MarkerOverhead { get => markerOverhead ?? throw new Exception(); set => markerOverhead = value; }
	[Export]
	private Node3D? markerOverhead;

	public Node3D MarkerCenterBody { get => markerCenterBody ?? throw new Exception(); set => markerCenterBody = value; }
	[Export]
	private Node3D? markerCenterBody;

	public Node3D MarkerBase { get => markerBase ?? throw new Exception(); set => markerBase = value; }
	[Export]
	private Node3D? markerBase;

	private Node3D? ModelScene;
	private AnimationPlayer? AnimationPlayer;

	private List<Vector3i> MovementStored = new();
	private EMovementMode MovementModeCurrent;

	private Dictionary<EMobSceneEffect, Node3D> EffectNodes = new();

	public override void _Ready()
	{
		base._Ready();
		if (MobUsing is null) throw new Exception("Lacks a MobUsing");

		EventBus.MobMoved += OnMobMoved;
		EventBus.MobStatChanged += OnMobStatChanged;
		EventBus.MobStatValueChanged += OnMobStatValueChanged;
		EventBus.MobTurnEnded += OnMobTurnEnded;
		EventBus.MobSelected += OnMobSelected;
		EventBus.ActionUsed += OnActionUsed;
		EventBus.InventoryChanged += OnInventoryChanged;
		EventBus.InputCheatEntered += OnInputCheatEntered;

		MobUsing.TemplateUpdate(true, true);
		MobUsing.EquipmentStatBoostsUpdate();

		MovementResetPosition();
	}


	public override void _Process(double delta)
	{
		base._Process(delta);
		MovementProcess();
	}

	#region Particles

	public void AnimatePopupText(string text, Godot.Color? color = null, Godot.Gradient? gradient = null)
	{
		PopupText3D popupText = Readonly.Scenes.SCENE_PARTICLE_POPUP_TEXT;
		popupText.Text = text;
		popupText.Color = color ?? Colors.White;
		popupText.ColorRamp = gradient;
		popupText.Finished += popupText.QueueFree;

		GetTree().Root.AddChild(popupText);

		popupText.GlobalPosition = MarkerOverhead.GlobalPosition;
		popupText.Emitting = true;
	}

	public void AnimateDialogue(string text)
	{
		DialogueBubble bubble = Readonly.Scenes.DIALOGUE_BUBBLE;
		bubble.SetText(text);
		MarkerOverhead.AddChild(bubble);
	}

	public void AnimateDialogueChance(List<string> text, float chance)
	{
		if (chance > 1) throw new Exception("Chance must be within 0 and 1");
		if (text.Count == 0) throw new Exception();

		if (chance >= GD.Randf())
			AnimateDialogue(text.GetRandom());
	}

	public void ToggleEffect(EMobSceneEffect effect, bool enabled)
	{
		Node3D? effectNode = EffectNodes.ContainsKey(effect) ? EffectNodes[effect] : null;
		Node3D parentNode = MarkerCenterBody;
		if (enabled)
		{
			effectNode?.QueueFree();
			effectNode = effect switch
			{
				EMobSceneEffect.TURN_ACTIVE
					=> Global.ManagerParticle
					.ResourceGet("HoveringStar")
					.Instantiate<Node3D>(),

				EMobSceneEffect.TARGETED
					=> Global.ManagerParticle
					.ResourceGet("InwardArrows")
					.Instantiate<Node3D>(),

				_
					=> throw new Exception("Invalid effect.")
			};

			parentNode = effect switch
			{
				EMobSceneEffect.TURN_ACTIVE => MarkerOverhead,
				EMobSceneEffect.TARGETED => MarkerCenterBody,
				_ => MarkerOverhead

			};

			parentNode.AddChild(effectNode);
			EffectNodes[effect] = effectNode;
		}
		else
		{
			effectNode?.QueueFree();
			EffectNodes.Remove(effect);
		}
	}


	#endregion

	#region Model
	public void SetBodyModel(EMobSceneBodyModel body)
	{
		MarkerCenterBody.FreeChildren();

		Node3D modelScene;
		switch (body)
		{
			case EMobSceneBodyModel.HUMAN:
				modelScene = Global.ManagerModel.GetInstance("BodyHuman");
				break;

			default: throw new Exception();
		}

		ModelScene = modelScene;
		MarkerCenterBody.AddChild(ModelScene);
	}

	#endregion

	#region Movement
	public void MovementProcess()
	{
		//Reached the end of the list, we are done.
		if (MovementStored.Count == 0)
		{
			MovementVerifyPosition();
			return;
		}

		Vector3i currentGoal = MovementStored[0];

		//Perform the movement
		Godot.Vector3 currentGoalGlobal = CombatScene.GetGridNode().MapToGlobal(currentGoal);
		switch (MovementModeCurrent)
		{
			case EMovementMode.GROUNDED:
				//Move to the goal
				GlobalPosition = GlobalPosition.MoveToward(currentGoalGlobal, MovementGetSpeed());
				break;
			
			case EMovementMode.PLACE:
				GlobalPosition = currentGoalGlobal;
				break;

			case EMovementMode.FLY:
				if (GlobalPosition.DistanceTo(currentGoalGlobal) > 2)
				{
					GlobalPosition = new(
						Mathf.MoveToward(GlobalPosition.X, currentGoalGlobal.X, MovementGetSpeed()),
						Mathf.MoveToward(GlobalPosition.Y, currentGoalGlobal.Y + 1, MovementGetSpeed()),
						Mathf.MoveToward(GlobalPosition.Z, currentGoalGlobal.Z, MovementGetSpeed())
					);
				}
				else
				{
					GlobalPosition = GlobalPosition.MoveToward(currentGoalGlobal, MovementGetSpeed());
				}
				break;

			default:
				throw new NotImplementedException($"Movement for {MovementModeCurrent} not implemented yet.");
		}


		//If close enough, change goal.
		if (Mathf.IsZeroApprox(GlobalPosition.DistanceTo(currentGoalGlobal)))
		{
			var popped = MovementStored.PopFirst();

			if (popped != currentGoal) throw new Exception("This was supposed to remove the current goal.");
		}
	}

	private float MovementGetSpeed()
	{
		float agility = Mathf.Clamp(MobUsing.Stats.GetStat(EStatName.AGILITY), 0, 200);
		return 0.8f * (agility / 100);
	}

	public void MovementResetPosition()
	{
		Godot.Vector3 vector = CombatScene.GetGridNode().MapToGlobal(MobUsing.GetPosition());
		GlobalPosition = vector;
	}

	public void MovementVerifyPosition()
	{
		if (!Mathf.IsZeroApprox(CombatScene.GetGridNode().MapToGlobal(MobUsing.GetPosition()).DistanceTo(GlobalPosition)))
		{
			throw new Exception($"Position mismatch. Node:{GlobalPosition} | Mob:{MobUsing.GetPosition()}");
		}
	}
	#endregion

	#region Event Handling
	private void OnMobMoved(Mob mob, List<Vector3i> path, MovementParameters moveParams)
	{
		if (mob != MobUsing) return;
		MovementModeCurrent = moveParams.MovementMode;
		MovementStored.AddRange(path);
	}

	private void OnMobStatChanged(Mob mob, EStatName stat, float change)
	{
		if (mob != MobUsing) return;
		string text;
		Godot.Color color = Colors.White;
		if (change < 0)
		{
			color = Colors.Red;
		}
		else if (change > 0)
		{
			color = Colors.Green;
		}

		color.A = 0.7f;
		AnimatePopupText($"{stat}: {change}", color);
	}

	private void OnMobStatValueChanged(Mob mob, EValueName stat, float change)
	{
		if (mob != MobUsing) return;
		string text;
		Godot.Color color = Colors.White;
		if (change < 0)
		{
			color = Colors.Red;
		}
		else if (change > 0)
		{
			color = Colors.Green;
		}

		switch (stat)
		{
			case EValueName.HEALTH:
				text = change.ToString();
				break;

			default: break;
		}
		AnimatePopupText($"{stat}: {change}", color);
	}

	private void OnMobTurnEnded(Mob mob)
	{
		if (mob != MobUsing) return;
		mob.TurnActive = false;
		ToggleEffect(EMobSceneEffect.TURN_ACTIVE, false);
	}

	private void OnMobSelected(Mob mob)
	{
		if (mob != MobUsing) return;

	}

	private void OnActionUsed(UsageParameters parameters)
	{
		if (parameters.OwnerRef != MobUsing) return;
		if (MobUsing.TurnActive)
			MobUsing.Stats.ChangeValue(EValueName.ACTION, -parameters.ActionRef.CostParams.Action);
		else
			MobUsing.Stats.ChangeValue(EValueName.REACTION, -parameters.ActionRef.CostParams.Reaction);

		MobUsing.Stats.ChangeValue(EValueName.SUB_ACTION, -parameters.ActionRef.CostParams.Move);

		MobUsing.Stats.ChangeValue(EValueName.MOVE, -parameters.ActionRef.CostParams.Move);
	}

	private void OnInventoryChanged(MobEquipmentInventory obj)
	{
		if (obj != MobUsing.EquipmentInventory) return;

		MobUsing.EquipmentStatBoostsUpdate();
	}

	private void OnInputCheatEntered(Command.ECheat obj)
	{
		if (!MobUsing.IsInCombat()) return;

		switch (obj)
		{
			case Command.ECheat.ALL_HP_TO_ONE:
				MobUsing.Stats.SetValue(EValueName.HEALTH, 1);
				break;

			case Command.ECheat.ALL_HP_FULL:
				MobUsing.Stats.RefillValues([EValueName.HEALTH]);
				break;

			case Command.ECheat.TEST_DIALOGUE_BUBBLES:
				AnimateDialogue("What did you say about me you little shit!\nYou shit!\nshit!\nwhat?");
				break;

			default:
				break;
		}
	}
    #endregion

}
