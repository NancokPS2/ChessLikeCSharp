using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Parameters;
using ChessLike.Extension;
using Godot;

namespace ChessLike.StatusEffect;

[GlobalClass]
public partial class Status : Resource
{
	public Mob? TargetMob
	{
		get => targetMob;
		set
		{
			targetMob = value;
		}
	}
	private Mob? targetMob;

	public bool Enabled = true;

	[Export]
	public string Name = "Unnamed Status.";

	[Export]
	protected string Description = "Undefined description."; 

	[Export]
	public Texture2D? Icon;

	public List<EStatusFlag> Flags = new();

	[Export]
	private Godot.Collections.Array<EStatusFlag> flags
	{
		set => Flags = new(flags);
		get => new(Flags);
	}

	[Export]
	protected AutoActivationParameters AutoParams = new();

	public int ActivationsLeft { protected set; get; }

	/// <summary>
	/// Stores the UsageParameters of an action during the reaction to it. Then it is disposed.
	/// </summary>
	public UsageParameters? ReactedToUsageParams { private set; get; }

	public virtual void Setup(Mob mob)
	{
		TargetMob = mob;
		EventBus.MobTurnEnded += OnMobTurnEnded;
		EventBus.MobTurnStarted += OnMobTurnStarted;
		EventBus.ActionPreUsed += OnActionPreUsed;
		EventBus.ActionUsed += OnActionUsed;
		ResetActivationCounter(AutoParams.AutoActivationMode);
	}

	public virtual void UnSetup()
	{
		EventBus.MobTurnEnded -= OnMobTurnEnded;
		EventBus.MobTurnStarted -= OnMobTurnStarted;
		EventBus.ActionPreUsed -= OnActionPreUsed;
		EventBus.ActionUsed -= OnActionUsed;
		TargetMob?.RemoveStatusEffect(this);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		UnSetup();
	}

	protected virtual void Use()
	{
		EventBus.StatusEffectUsed?.Invoke(TargetMob, this);
	}

	public virtual string GetDescription(bool includeBase = true)
	{
		string output = "";
		string activationText = "";
		switch (AutoParams.AutoActivationMode)
		{
			case EAutoActivationMode.TURN_CHANGE:
				string targetOrAnyTurnChange = AutoParams.ActivatedOnlyIfTurnIsMine ? "the afflicted" : "someone";
				string endOrStart = AutoParams.ActivatedByTurnEnd ? "ends their turn" : "starts their turn";
				activationText = $"Triggers whenever {targetOrAnyTurnChange} {endOrStart}.";
				break;

			case EAutoActivationMode.ACTION_REACTION:
				string flags = AutoParams.ActivatedByActionWithFlags.ToStringList(", ");
				string targetOrAnyActionReaction = AutoParams.ActivatedOnlyIfTargetsMe ? "yourself" : "someone";
				activationText = $"Triggers whenever an ability with the {flags} properties is used on {targetOrAnyActionReaction}.";
				break;

			case EAutoActivationMode.EVERY_X_TIME:
				activationText = $"Triggers whenever {AutoParams.ActivatedEveryXTime} time passes.";
				break;
			
			default: throw new Exception();
		}

		if (activationText != "")
			output = output.NewLine(activationText);

		output = output.NewLine(Description);
		return output;
	}

	protected void ResetActivationCounter(EAutoActivationMode mode)
		=> ActivationsLeft = AutoParams.AutoActivationMode switch
		{
			EAutoActivationMode.NONE => throw new Exception(),
			_ => AutoParams.AutoActivationMax
		};

	private bool IsValidToUse()
	{
		if (TargetMob is null) throw new Exception();
		if (ActivationsLeft < 1) return false;
		return true;
	}

	protected virtual bool IsValidForTurnEnd(Mob mob)
	{
		if (!IsValidToUse()) return false;
		if (AutoParams.AutoActivationMode != Entity.Action.Parameters.EAutoActivationMode.TURN_CHANGE) return false;
		if (!AutoParams.ActivatedByTurnEnd) return false;
		if (AutoParams.ActivatedOnlyIfTurnIsMine && (mob != TargetMob)) return false;

		return true;
	}

	protected virtual bool IsValidForTurnStart(Mob mob)
	{

		if (!IsValidToUse()) return false;
		if (AutoParams.AutoActivationMode != Entity.Action.Parameters.EAutoActivationMode.TURN_CHANGE) return false;
		if (AutoParams.ActivatedByTurnEnd) return false;
		if (AutoParams.ActivatedOnlyIfTurnIsMine && (mob != TargetMob)) return false;

		return true;
	}

	protected virtual bool IsValidForActionActivation(UsageParameters parameters, bool after)
	{
		if (!IsValidToUse()) return false;
		if (AutoParams.AutoActivationMode != EAutoActivationMode.ACTION_REACTION) return false;
		if (AutoParams.IsActionWithValidFlags(parameters.ActionRef)) return false;
		if (AutoParams.ActivatedAfterAction && !after) return false;
		if (AutoParams.ActivatedOnlyIfTargetsMe && !parameters.MobsTargeted.Contains(TargetMob)) return false;

		return true;
	}

	public bool IsBuff() => Flags.Contains(EStatusFlag.BUFF);
	public bool IsDebuff() => Flags.Contains(EStatusFlag.DEBUFF);


	#region Event Handling
	protected virtual void OnMobTurnStarted(Mob mob)
	{
		if (!IsValidForTurnStart(mob)) return;
		ActivationsLeft--;
		Use();
	}

	protected virtual void OnMobTurnEnded(Mob mob)
	{
		//Remove all used up status effects on turn end.
		if (!IsValidToUse())
		{
			UnSetup();
			return;
		}

		if (!IsValidForTurnEnd(mob)) return;
		ActivationsLeft--;
		Use();
	}

	protected void OnActionUsed(UsageParameters parameters)
	{
		if (!IsValidForActionActivation(parameters, true)) return;
		ActivationsLeft--;

		ReactedToUsageParams = parameters;
		Use();
		ReactedToUsageParams = null;
	}

	private void OnActionPreUsed(UsageParameters parameters)
	{
		if (!IsValidForActionActivation(parameters, false)) return;
		ActivationsLeft--;

		ReactedToUsageParams = parameters;
		Use();
		ReactedToUsageParams = null;
	}
	#endregion
}
