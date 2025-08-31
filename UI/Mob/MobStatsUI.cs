using ChessLike.Entity;
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
		foreach (var item in mob.Stats.AllStats)
		{
			float current = mob.Stats.HasValueAssociatedToStat(item) ? mob.Stats.GetValueByStat(item) : float.MinValue;
			float max = mob.Stats.GetStat(item);

			string text = item.ToString() + ": ";
			if (current == max || current == float.MinValue) { text += max.ToString(); }
			else { text += current.ToString() + "/" + max.ToString(); }

			StatsLabel label = new(mob.Stats, item) { Text = text, SizeFlagsHorizontal = SizeFlags.ExpandFill };
			NodeStatContainer.AddChild(label);
		}

		NodeStatusContainer.FreeChildren();
		foreach (var item in mob.GetAllStatusEffects())
		{
			NodeStatusContainer.AddChild(
				new Label(){Text = item.Name}
			);
		}

	}

    private partial class StatsLabel : Label, ITooltip
	{
		public MobStatSet StatSet;
		public EStatName Stat;

		public StatsLabel(MobStatSet stat_set, EStatName stat)
		{
			StatSet = stat_set;
			Stat = stat;
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
