using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity;

public partial class MobSceneManager : Node3D
{
	[Export]
	protected double DialogueIdleChatterMinTime = 5;

	[Export]
	protected double DialogueIdleChatterMaxTime = 30;

	[Export]
	protected float DialogueReactionMaxChance = 0.8f;

	[Export]
	protected float DialogueIdleMaxChance = 0.6f;

	protected double DialogueTimePassed;
	protected double DialogueTimeThreshold;
	protected void ConnectDialogue()
	{
		EventBus.MobTurnStarted += OnMobTurnStartedDialogue;
		EventBus.ActionUsed += OnActionUsed;
	}

	protected void DialogueProcess(double delta)
	{
		if (InstancedMobs.Count == 0) return;
		if (DialogueTimePassed > DialogueTimeThreshold)
		{
			IdleChatterProc();
			IdleChatterReset();
		}

		DialogueTimePassed += delta;
	}

	protected void IdleChatterReset()
	{
		DialogueTimeThreshold = GD.RandRange(DialogueIdleChatterMinTime, DialogueIdleChatterMaxTime);
		DialogueTimePassed = 0;
	}

	protected void IdleChatterProc()
	{
		MobScene instance = InstancedMobs.GetRandom() ?? throw new Exception();
		instance.AnimateDialogueChance(
			["I'm waiting!", "We got this, i got this.", "What a face-off.", "This isn't over yet."],
			DialogueIdleMaxChance
		);
	}

	private void OnActionUsed(UsageParameters parameters)
	{
		List<MobScene> instances = new();
		for (int i = 0; i < InstancedMobs.Count; i += 2)
		{
			instances.Add(InstancedMobs.GetRandom() ?? throw new Exception());
		}
		foreach (var instance in instances)
		{
			List<string> texts = new();
			Faction instanceFaction = instance.MobUsing.GetFaction();

			bool fromInstance = instance.MobUsing == parameters.OwnerRef;
			bool fromAlly = !fromInstance && parameters.OwnerRef.GetFaction().IsAlly(instance.MobUsing.Faction);
			bool fromEnemy = !fromInstance && !fromAlly && parameters.OwnerRef.GetFaction().IsEnemy(instance.MobUsing.Faction);

			bool isHarmful = parameters.ActionRef.Flags.Contains(EActionFlag.HOSTILE);
			bool isHelpful = parameters.ActionRef.Flags.Contains(EActionFlag.HEALING);

			bool isTargetingAlly = parameters.MobsTargeted.All(x => instanceFaction.IsAlly(x.Faction));
			bool isTargetingEnemy = parameters.MobsTargeted.All(x => instanceFaction.IsEnemy(x.Faction));
			bool isTargetingInstance = parameters.MobsTargeted.Contains(instance.MobUsing);


			if (fromAlly && isHelpful && isTargetingInstance)
				texts = ["Thank you.", "That helps."];

			else if (fromEnemy && isHarmful && isTargetingAlly)
				texts = ["Those bastards."];

			else if (fromEnemy && isTargetingInstance && isHarmful)
				texts = GetDamageText(instance.MobUsing.Stats.GetValuePrecent(EValueName.HEALTH));

			else if (fromAlly && isTargetingInstance && isHarmful)
				texts = ["Watch it!", "Hit THEM, not me!", "Stop! We are on the same side!"];

			if (texts.Count == 0) return;

			instance.AnimateDialogueChance(
				texts,
				DialogueReactionMaxChance
			);
		}
	}

	private static List<string> GetDamageText(float percentLeft)
	{
		List<string> texts = new();
		if (percentLeft > 0.9) texts.AddRange(["Ugh!", "Ack!", "Gah!"]);
		if (percentLeft > 0.5) texts.AddRange(["I'm... wounded...", "I need help!"]);
		if (percentLeft > 0.2) texts.AddRange(["That stings!", "Fair hit..."]);
		else texts.AddRange(["That's all!?", "I barely felt it.", "I've dealt with worse."]);
		return texts;
	}


	private void OnMobTurnStartedDialogue(Mob mob)
	{
		GetInstance(mob)?.AnimateDialogueChance(["Ready!", "My turn!", "Action!"], 1);
	}


	
}
