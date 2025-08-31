using ChessLike.Entity;
using ChessLike.StatusEffect;
using ExtendedXmlSerializer;
using Godot;
using System;

[GlobalClass]
public partial class MobStatsUI : BaseMobUI
{
	[Export]
	Control? NodeStatContainer;

	[Export]
	Control? NodeStatusContainer;

	protected override void Update(Mob mob)
	{
		base.Update(mob);

		if (NodeStatContainer is null) throw new Exception("Null NodeStatContainer");
		if (NodeStatusContainer is null) throw new Exception("Null NodeStatusContainer");

		NodeStatContainer.FreeChildren();
		foreach (var statName in mob.Stats.AllStats)
		{
			float current = mob.Stats.HasValueAssociatedToStat(statName) ? mob.Stats.GetValueByStat(statName) : float.MinValue;
			float max = mob.Stats.GetStat(statName);

			string text = statName.ToString() + ": ";
			if (current == max || current == float.MinValue) { text += max.ToString(); }
			else { text += current.ToString() + "/" + max.ToString(); }

			NodeStatContainer.AddChild(
				new StatsLabel(mob.Stats, statName) { Text = text }
				);
		}

		NodeStatusContainer.FreeChildren();
		foreach (var status in mob.GetAllStatusEffects())
		{
			NodeStatusContainer.AddChild(
				new StatusLabel(status){Text = $"{status.Name} - {status.ActivationsLeft}"}
			);
		}

	}

	private partial class StatusLabel : Label, ITooltip
	{
		public Status StatusEffect;

		public StatusLabel(Status status)
		{
			StatusEffect = status;
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
		}

		string ITooltip.GetText()
		{
			return StatusEffect.GetDescription();
		}

		Godot.Vector2 ITooltip.GetRectSize() => new(200, 80);

	}

    private partial class StatsLabel : Label, ITooltip
	{
		public MobStatSet StatSet;
		public EStatName Stat;

		public StatsLabel(MobStatSet stat_set, EStatName stat)
		{
			StatSet = stat_set;
			Stat = stat;
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
		}
		string ITooltip.GetText()
		{
			string output = $"{Enum.GetName(Stat) ?? throw new Exception()}\n"
			+ $"{StatSet.BoostGetListOfStatChanges(Stat)}";

			return output;
		}

		Godot.Vector2 ITooltip.GetRectSize() => new(200, 80);
	}
}
