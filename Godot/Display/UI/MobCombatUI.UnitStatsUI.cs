using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;

namespace Godot.Display;

public partial class UnitStatsUI
{
    public static readonly NodeRequirement NAME_LABEL = new(
        "NAME_LABEL",
        typeof(Label)
        );
    public static readonly NodeRequirement DELAY_LABEL = new(
        "DELAY_LABEL",
        typeof(Label)
        );
    public static readonly NodeRequirement HEALTH_BAR = new(
        "HEALTH_BAR",
        typeof(ProgressBar)
        );
    public static readonly NodeRequirement ENERGY_BAR = new(
        "ENERGY_BAR",
        typeof(ProgressBar)
        );

    public ItemList<object> StatList;

    public UnitStatsUI(Control stat_list_ref)
    {
        StatList = new(stat_list_ref);
    }

    private Mob? _owner_of_stats;
    public void UpdateStatNodes(Mob mob)
    {
        _owner_of_stats = mob;

        StatList.ControlReference.GetNodeFromRequirement<Label>(NAME_LABEL)
            .Text = mob.DisplayedName;

        StatList.ControlReference.GetNodeFromRequirement<Label>(DELAY_LABEL)
            .Text = mob.Stats.GetValue(StatName.DELAY).ToString();

        StatList.ControlReference.GetNodeFromRequirement<ProgressBar>(HEALTH_BAR)
            .Value = mob.Stats.GetValue(StatName.HEALTH);

        StatList.ControlReference.GetNodeFromRequirement<ProgressBar>(HEALTH_BAR)
            .MaxValue = mob.Stats.GetMax(StatName.HEALTH);

        StatList.ControlReference.GetNodeFromRequirement<ProgressBar>(ENERGY_BAR)
            .Value = mob.Stats.GetValue(StatName.ENERGY);

        StatList.ControlReference.GetNodeFromRequirement<ProgressBar>(ENERGY_BAR)
            .MaxValue = mob.Stats.GetMax(StatName.ENERGY);
    }
    public Mob? GetOwnerOfStats()
    {
        return _owner_of_stats;
    }

}
