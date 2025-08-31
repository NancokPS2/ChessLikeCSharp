using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using ExtendedXmlSerializer;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEvent : Resource
{
	private Mob? owner;
	public Mob? Owner
	{
		get
		{
			if (owner is null) MsgLog.LogErrorMsg($"Missing owner for ability {Name}!");
			return owner;
		}
		set
		{
			owner = value;
		}
	}

	[Export]
	public string Name = "Undefined Action";

	[Export]
	private Godot.Collections.Array<EActionFlag> flags
	{
		set => Flags = new(value);
		get => new(Flags);
	}
	public List<EActionFlag> Flags = new();

	[Export]
	private Godot.Collections.Array<MobCommand.Command> commands
	{
		set => Commands = new(value);
		get => new(Commands);
	}
	public List<MobCommand.Command> Commands = new();

	[ExportGroup("Parameters")]

	[Export]
	public TargetingParameters TargetParams = new();

	[Export]
	public AnimationParameters AnimationParams = new();

	[Export]
	public MobFilterParameters MobFilterParams = new();

	[Export]
	public CostParameters CostParams = new();


	public ActionEvent()
	{
		EventBus.InputActionSelected += OnInputActionSelected;
	}

	public virtual void Setup(Mob mob)
	{
		Owner = mob;
	}


	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	#region Visual
	public Texture2D GetAnimationFloatingTexture() => throw new NotImplementedException();

	public virtual void AnimationRun(UsageParameters parameters)
	{
		if (parameters.ActionRef != this) throw new Exception();
	}

	#endregion

	#region Targeting
	public int GetMaxTargetingSelections()
		=> TargetParams.MaxPositions;

	public uint GetTotalRange(Mob owner)
	{
		uint output = TargetParams.Range;
		if (TargetParams.RangeStatBonus is EStatName stat && stat != EStatName.NONE)
		{
			output += (uint)owner.Stats.GetStat(stat);
		}
		return output;
	}

	/// <summary>
	/// Returns the positions targetable by this action, relative to the owner.
	/// </summary>
	/// <param name="usageParams">The grid and owner are drawn from this</param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public List<Vector3i> GetTargetVectors(UsageParameters usageParams)
	{
		if (!usageParams.IsValid()) throw new Exception();
		if (usageParams.ActionRef != this) throw new Exception();
		if (usageParams.OwnerRef != Owner) throw new Exception();

		if (Owner is null)
			throw new Exception("Owner missing!");

		Vector3i origin = usageParams.OwnerRef.GetPosition();
		Grid grid = usageParams.GridRef;
		Mob owner = usageParams.OwnerRef;
		List<Vector3i> output = new();

		//If it uses pathing, just query that directly and move on.
		if (TargetParams.TargetingUsesPathing != EMovementMode.INVALID)
		{
			output = CombatScene.GetMobMovement().GetPathablePositions(owner, TargetParams.TargetingUsesPathing);
			return output;
		}

		//Get the shape.
		output = TargetParams.GetTargetingShape();

		//Convert the list to be relative to its owner position.
		output = output.Select(x => x + Owner.GetPosition()).ToList();

		//Make sure they are inbounds
		output = output
			.Where(x => grid.IsPositionInbounds(x))
			.ToList();

		//Select positions within range and filter them.
		uint maxRange = GetTotalRange(owner);
		output = output
			.Where(x => x.DistanceManhattanTo(origin) <= maxRange)
			.ToList();

		return output;
	}

	/// <summary>
	/// It must have selected positions before being used
	/// </summary>
	/// <param name="usageParams"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public List<List<Vector3i>> GetAffectedVectors(UsageParameters usageParams)
	{
		//Error checks
		if (!usageParams.IsValid()) throw new Exception();

		if (usageParams.ActionRef != this) throw new Exception();
		if (usageParams.OwnerRef != Owner) throw new Exception();
		if (usageParams.PositionsTargeted.Count == 0) throw new Exception("No position to use AoE in.");

		if (Owner is null)
			throw new Exception("Owner missing!");



		Vector3i origin = usageParams.OwnerRef.GetPosition();
		Grid grid = usageParams.GridRef;
		Mob owner = usageParams.OwnerRef;
		List<List<Vector3i>> output = new();

		//Pathing based AoE
		if (TargetParams.TargetingUsesPathing != EMovementMode.INVALID && TargetParams.AoEUsesPathing == EMovementMode.INVALID)
			throw new Exception("I don't know how to handle the Targeting using Pathing but not the AoE!");

		if (TargetParams.AoEUsesPathing != EMovementMode.INVALID)
		{
			if (usageParams.PositionsTargeted.Count != 1)
				MsgLog.Log(EMessageType.ERROR, $"{Name} uses pathing but also used {usageParams.PositionsTargeted.Count} targets, can't move to multiple locations!");

			CombatScene.GetMobMovement()
				.GetAStar(Owner, TargetParams.TargetingUsesPathing)
				.GetPath(Owner.GetPosition(), usageParams.PositionsTargeted.First());
		}

		//Regular AoE checks
		foreach (Vector3i selected in usageParams.PositionsTargeted)
		{
			List<Vector3i> subOutput = new();

			//Get the rotation to look from the owner to the target. This is relative to the user, so ZERO atm.
			Vector3i.Rotation rotation = Vector3i.ZERO.GetRotationToLookAt(selected, true);

			//Get the shape.
			subOutput = TargetParams.GetAoEShape(rotation);

			//Convert the list to be relative to the selection position.
			subOutput = subOutput.Select(x => x + selected).ToList();

			//Make sure they are inbounds
			subOutput = subOutput
				.Where(x => grid.IsPositionInbounds(x))
				.ToList();

			//Select positions within range and filter them.
			uint maxRange = GetTotalRange(owner);
			subOutput = subOutput
				.Where(x => x.DistanceManhattanTo(origin) <= maxRange)
				.ToList();

			//Add new entries that are not duplicated to output.
			foreach (var cluster in output)
			{
				subOutput = subOutput
					.Where(x => cluster.Contains(x))
					.ToList();
			}

			output.Add(subOutput);
		}

		return output;
	}



	#endregion

	#region Mob Filter
	public List<Mob> GetValidMobs(List<Mob> mobPositions)
		=> mobPositions.Where(x => IsMobValidForAoE(x)).ToList();

	public bool IsMobValidForAoE(Mob mob)
	{
		Faction owner_fac = Global.ManagerFaction.ResourceGet(EPackIDFaction.Player);

		//Can affect owner?
		if (mob == Owner && MobFilterParams.CannotAffectOwner)
		{
			return false;
		}
		//If health is above the max percent, fail.
		else if (mob.Stats.GetValuePrecent(EValueName.HEALTH) > MobFilterParams.MaximumHealthPercent)
		{
			return false;
		}
		//Check for faction
		else if (owner_fac.IsAlly(mob.Faction) && MobFilterParams.CannotAffectAlly)
		{
			return false;
		}
		else if (owner_fac.IsEnemy(mob.Faction) && MobFilterParams.CannotAffectEnemy)
		{
			return false;
		}
		return true;
	}
	#endregion
	/* 
		#region  Auto Activation
		[Export]
		protected AutoActivationParameters AutoActivationParams
		{
			get => autoActivationParams;
			set
			{
				autoActivationParams = value;
				AutoActivationReset();
			}
		}
		private AutoActivationParameters autoActivationParams = new();

		protected bool AutoActivationEnabled = true;

		protected float AutoActivationTimeSinceLast;

		protected int AutoActivationLeft;

		[Obsolete("Defer all this to status effects.")]
		private void AutoActivationSetup()
		{
			EventBus.ActionQueued += OnActionQueued;

			EventBus.TurnTimePassed += OnAutoActivationProcessTimePassed;

			EventBus.MobTurnStarted += OnAutoActivationProcessTurnStarted;
			EventBus.MobTurnEnded += OnAutoActivationProcessTurnEnded;
		}

		protected int GetAutoActivationsLeft() => AutoActivationLeft;

		protected void AutoActivationReset()
		{
			AutoActivationLeft = AutoActivationParams.AutoActivationMax;
			AutoActivationTimeSinceLast = 0;
		}

		protected virtual UsageParameters GetAutoActivationUsageParametersFromReaction(UsageParameters parameters)
			=> new(
				Owner ?? throw new Exception("Owner missing!"),
				parameters.GridRef,
				this);
				//{ PositionsTargeted = new(){Owner.GetPosition()}};
		protected virtual UsageParameters GetAutoActivationUsageParameters()
			=> new(
				Owner ?? throw new Exception("Owner missing!"),
				CombatScene.GetGrid(),
				this);
				//{ PositionsTargeted = new(){Owner.GetPosition()}};

		protected void AutoActivationRequest(UsageParameters parameters)
		{
			//Can't activate if it ran out.
			if (AutoActivationLeft <= 0)
			{
				throw new Exception("Should already be removed?");
			}

			EventBus.ActionEventAutoActivated?.Invoke(
				GetAutoActivationUsageParametersFromReaction(parameters),
				parameters
				);
			AutoActivationLeft -= 1;
			AutoActivationTimeSinceLast = 0;
		}
		#endregion
	 */
	#region General
	public virtual string GetUseText(UsageParameters parameters)
	{
		return $"{Owner.DisplayedName ?? "ERROR"} did something mysterious to {parameters.MobsTargeted.ToStringList(", ")}";
	}

	public virtual void Use(UsageParameters usageParams)
	{
		if (usageParams.ActionRef != this)
			throw new Exception();

		if (!usageParams.HasPositionsTargeted())
			throw new Exception();

		if (Owner is null)
				throw new Exception();

		foreach (var command in Commands)
			{
				foreach (var mob in usageParams.MobsTargeted)
				{
					command.UseCommand(mob);
				}
			}
		EventBus.ActionUsed?.Invoke(usageParams);
	}

	public static bool HasMobEnoughResources(ActionEvent action, Mob mob)
	{
		bool enoughActions = mob.Stats.GetValue(EValueName.ACTION) >= action.CostParams.Action;
		bool enoughSubAction = mob.Stats.GetValue(EValueName.MOVE) >= action.CostParams.SubAction;
		bool enoughReactions = mob.Stats.GetValue(EValueName.ACTION) >= action.CostParams.Reaction;
		bool enoughMoves = mob.Stats.GetValue(EValueName.MOVE) >= action.CostParams.Move;
		return enoughActions && enoughSubAction && enoughReactions && enoughMoves;
	}

	public bool CanUse() => true;
	public override string ToString() => Name;

	public void ThrowOnMissingOwner()
	{
		if (Owner is null)
			throw new Exception();
	}

	#endregion

	#region Event Handling
/* 
	protected void OnAutoActivationProcessTurnStarted(Mob who)
	{
		//Make sure it has an owner
		if (Owner is null)
			throw new Exception("Owner missing!");

		//Must be enabled to begin with.
		if (!AutoActivationEnabled) return;
		//MsgLog.LogInfoMsg($"{Name} could not activate since it has auto activation disabled.");

		//Check if it activates on turn end or start.
		if (!AutoActivationParams.ActivatedByTurnStart) return;

		//Must be set to react to actions
		if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.TURN_CHANGE) return;

		//Make sure an owner is set
		if (Owner is null)
		{
			MsgLog.LogErrorMsg($"Ability {Name}, which was just auto activated, does not have an owner.");
			return;
		}

		//The owner must be in combat.
		if (!Owner.IsInCombat()) return;

		//If it is only when THIS unit's turn changes, do nothing if false.
		if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && who != Owner) return;

		AutoActivationRequest(GetAutoActivationUsageParameters());
	}

	protected void OnAutoActivationProcessTurnEnded(Mob who)
	{
		//Make sure it has an owner
		if (Owner is null)
			throw new Exception("Owner missing!");

		//Must be enabled to begin with.
		if (!AutoActivationEnabled) return;

		//Check if it activates on turn end or start.
		if (!AutoActivationParams.ActivatedByTurnEnd) return;

		//Must be set to react to actions
		if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.TURN_CHANGE) return;

		//Make sure an owner is set
		if (Owner is null)
		{
			MsgLog.LogErrorMsg($"Ability {Name}, which was just auto activated, does not have an owner.");
			return;
		}

		//The owner must be in combat.
		if (!Owner.IsInCombat()) return;

		//If it is only when THIS unit's turn changes, do nothing if false.
		if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && who != Owner) return;

		AutoActivationRequest(GetAutoActivationUsageParameters());
	}

	protected void OnAutoActivationProcessTimePassed(float number)
	{
		//Make sure it has an owner
		if (Owner is null)
			throw new Exception("Owner missing!");

		//Must be enabled to begin with.
		if (!AutoActivationEnabled) return;

		//Must be set to react to actions
		if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.EVERY_X_TIME) return;

		//Make sure an owner is set
		if (Owner is null)
		{
			MsgLog.LogErrorMsg($"Ability {Name}, which was just auto activated, does not have an owner.");
			return;
		}

		//The owner must be in combat.
		if (!Owner.IsInCombat()) return;

		//Advance time.
		AutoActivationTimeSinceLast += number;

		//If enough time passed, trigger.
		if (AutoActivationTimeSinceLast > AutoActivationParams.ActivatedEveryXTime)
			AutoActivationRequest(GetAutoActivationUsageParameters());

	}

	protected void OnActionQueued(UsageParameters parameters)
	{
		//Make sure it has an owner
		if (Owner is null)
			throw new Exception("Owner missing!");

		//Must be enabled to begin with.
		if (!AutoActivationEnabled) return;
		
		//Must be set to react to actions
		if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.ACTION_REACTION) return;

		//The owner must be in combat.
		if (!Owner.IsInCombat()) return;

		//Must have the right flags.
		if (!AutoActivationParams.IsActionWithValidFlags(parameters.ActionRef)) return;

		//Must be targeting the owner if the condition is true
		if (AutoActivationParams.ActivatedOnlyIfTargetsMe && !parameters.MobsTargeted.Contains(Owner)) return;

		AutoActivationRequest(GetAutoActivationUsageParametersFromReaction(parameters));
	}
 */
	private void OnInputActionSelected(ActionEvent obj)
	{
		if (obj != this) return;

		EventBus.TargetingUsageParametersGenerated?.Invoke(
			new(Owner ?? throw new Exception("Owner missing!"), CombatScene.GetGrid(), this)
		);
	}
	#endregion
}
