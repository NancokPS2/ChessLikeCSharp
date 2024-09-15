using ChessLike.Entity;
using Action = ChessLike.Entity.Action;

namespace Godot.Display;

public partial class MobUI : Control
{
    public delegate void ActionPressedEventHandler(Action action);
    public event ActionPressedEventHandler? ActionPressed;

    public const string SCENE_PATH = "res://assets/PackedScene/MobUI.tscn";

    static readonly NodeRequirement STAT_CONTAINER = new(
        "STAT_CONTAINER",
        typeof(VBoxContainer)
        );

    static readonly NodeRequirement ACTION_CONTAINER = new(
        "ACTION_CONTAINER",
        typeof(VBoxContainer)
        );

    static readonly NodeRequirement TURN_CONTAINER = new(
        "TURN_CONTAINER",
        typeof(HBoxContainer)
        );

    static readonly NodeRequirement NAME_LABEL = new(
        "NAME",
        typeof(Label)
        );
    static readonly NodeRequirement DELAY_LABEL = new(
        "DELAY_LABEL",
        typeof(Label)
        );
    static readonly NodeRequirement HEALTH_BAR = new(
        "HEALTH_BAR",
        typeof(ProgressBar)
        );
    static readonly NodeRequirement ENERGY_BAR = new(
        "ENERGY_BAR",
        typeof(ProgressBar)
        );
        
    public static readonly List<NodeRequirement> NodesRequired = new()
    {
        //Container
        ACTION_CONTAINER, STAT_CONTAINER, TURN_CONTAINER,
        //Label
        NAME_LABEL, DELAY_LABEL,
        //ProgressBar
        HEALTH_BAR, ENERGY_BAR,
    };

    public MobUI()
    {
    }

    private Mob? _owner_of_stats;
    public void UpdateStatNodes(Mob mob)
    {
        _owner_of_stats = mob;
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        this.GetNodeFromRequirement<Label>(NAME_LABEL)
            .Text = mob.DisplayedName;

        this.GetNodeFromRequirement<Label>(DELAY_LABEL)
            .Text = mob.Stats.GetValue(StatName.DELAY).ToString();

        this.GetNodeFromRequirement<ProgressBar>(HEALTH_BAR)
            .Value = mob.Stats.GetValue(StatName.HEALTH);

        this.GetNodeFromRequirement<ProgressBar>(HEALTH_BAR)
            .MaxValue = mob.Stats.GetMax(StatName.HEALTH);

        this.GetNodeFromRequirement<ProgressBar>(ENERGY_BAR)
            .Value = mob.Stats.GetValue(StatName.ENERGY);

        this.GetNodeFromRequirement<ProgressBar>(ENERGY_BAR)
            .MaxValue = mob.Stats.GetMax(StatName.ENERGY);
    }
    public Mob? GetOwnerOfStats()
    {
        return _owner_of_stats;
    }

    public void UpdateActionButtons(Mob mob)
    {
        foreach (Node node in this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER).GetChildren())
        {
            node.QueueFree();
        }
        
        foreach (ChessLike.Entity.Action action in mob.Actions)
        {
            ActionButton button = new(action);
            this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER)
                .AddChild(button);

            button.Text = action.name;
            Console.WriteLine(button.GetPath());
            button.Pressed += () => ActionPressed?.Invoke(action);
        }
    }

    public void EnableActionButtons(bool enable)
    {
        foreach (Node node in this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER).GetChildren())
        {
            if (node is Button button)
            {
                button.Disabled = !enable;
            }
        }
    }
    #pragma warning restore CS8602 // Dereference of a possibly null reference.


    private partial class ActionButton : Button
    {
        public Action action;

        public ActionButton(Action action)
        {
            this.action = action;
            Text = action.name;
        }
    }

}
