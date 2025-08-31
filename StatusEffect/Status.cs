using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
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
	public string Name = "Unnamed Status";


	[Export]
	protected Texture2D? Icon;

	public List<EStatusFlag> Flags = new();

	[Export]
	private Godot.Collections.Array<EStatusFlag> flags
	{
		set => Flags = new(flags);
		get => new(Flags);
	}

	[Export]
	protected AutoActivationParameters AutoActivationParams = new();

	public void Setup()
	{
		EventBus.MobTurnEnded += OnMobTurnEnded;
		EventBus.MobTurnStarted += OnMobTurnStarted;
	}

	public void UnSetup()
	{
		EventBus.MobTurnEnded -= OnMobTurnEnded;
		EventBus.MobTurnStarted -= OnMobTurnStarted;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		UnSetup();
	}

	protected virtual void Use()
	{

	}

	protected virtual bool IsValidForTurnEnd(Mob mob)
	{
		if (TargetMob is null) throw new Exception();
		if (AutoActivationParams.AutoActivationMode != Entity.Action.Parameters.EAutoActivationMode.TURN_CHANGE) return false;
		if (!AutoActivationParams.ActivatedByTurnEnd) return false;
		if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && (mob != TargetMob)) return false;

		return true;
	}
	protected virtual bool IsValidForTurnStart(Mob mob)
	{
		if (TargetMob is null) throw new Exception();
		if (AutoActivationParams.AutoActivationMode != Entity.Action.Parameters.EAutoActivationMode.TURN_CHANGE) return false;
		if (!AutoActivationParams.ActivatedByTurnStart) return false;
		if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && (mob != TargetMob)) return false;

		return true;
	}

	#region Event Handling
	protected virtual void OnMobTurnStarted(Mob mob)
	{
		if (!IsValidForTurnStart(mob)) return;
		Use();
	}

	protected virtual void OnMobTurnEnded(Mob mob)
	{
		if (!IsValidForTurnEnd(mob)) return;
		Use();
	}
	#endregion
}
