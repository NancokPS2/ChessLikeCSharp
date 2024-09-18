using ChessLike.Entity;
using ChessLike.Turn;
using Action = ChessLike.Entity.Action;

namespace Godot.Display;

public partial class MobUI : CanvasLayer
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
        "NAME_LABEL",
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
        string path = GetPath();

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

    public void UpdateDelayList(DelayManager manager)
    {
        HBoxContainer container = this.GetNodeFromRequirement<HBoxContainer>(TURN_CONTAINER);
        container.FreeChildren();

        foreach (var item in manager.GetParticipants())
        {
            container.AddChild(node: new DelayContainer(item));
        }
    }

    public void UpdateActionButtons(Mob mob)
    {
        VBoxContainer container = this.GetNodeFromRequirement<VBoxContainer>(ACTION_CONTAINER);
        
        foreach (ChessLike.Entity.Action action in mob.Actions)
        {
            ActionButton button = new(action);
            container.AddChild(button);

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

    private partial class DelayContainer : TextureRect
    {
        private readonly ITurn User;
        public DelayContainer(ITurn turn)
        {
            User = turn;
        }

        public override void _Ready()
        {
            base._Ready();
            SetAnchorsPreset(LayoutPreset.FullRect);
            SizeFlagsHorizontal = SizeFlags.ExpandFill;

            //Label and name
            string name = "";
            string delay = "0?";
            if (User is Mob mob)
            {
                name = mob.DisplayedName;
                delay = mob.DelayCurrent.ToString();
            }

            Label label_name = new();
            label_name.Text = name;
            AddChild(label_name);
            label_name.SetAnchorsPreset(LayoutPreset.TopWide);

            Label label_delay = new();
            label_delay.Text = delay;
            AddChild(label_delay);
            label_delay.SetAnchorsPreset(LayoutPreset.BottomWide);
        }

    }

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
