using ChessLike.Entity;
using Action = ChessLike.Entity.Action;

namespace Godot.Display.Interface;

public partial class MobUI : Control, IGSceneAdapter
{
    public delegate void ActionPressedEventHandler(Action action);
    public event ActionPressedEventHandler ActionPressed;

    public enum NodeType
    {
        STAT_CONTAINER, ACTION_CONTAINER, //VBoxContainer
        NAME, DELAY, //Label
        HEALTH, ENERGY, //ProgressBar
    }

    private Dictionary<NodeType, Control> nodes = new();
    private Mob mob;

    public List<IGSceneAdapter.NodeDeclaration> NodesRequired { get; set; }


    public MobUI(Mob mob)
    {
        SetMob(mob);
    }

    public override void _Ready()
    {
        base._Ready();
        if (!SetNodesFromChildren())
        {
            CreateAllNodes();
        }
    }


    /// <summary>
    /// Tries to find and sets the nodes to existing children. Useful for pre-made scenes.
    /// </summary>
    /// <returns>Wether it succeeded on finding the right nodes or not.</returns>
    public bool SetNodesFromChildren(NodeType[]? node_types = null)
    {
        //Check the specified types or ALL of them.
        NodeType[] types_to_check = node_types ?? Enum.GetValues<NodeType>();

        foreach (NodeType type in types_to_check)
        {
            string? expected_name = Enum.GetName(typeof(NodeType), type) ?? throw new Exception("This should be impossible.");

            Control node = (Control)GetNodeOrNull(expected_name);

            if (node is Control)
            {
                nodes[type] = node; 
            } else
            {
                return false;
            }
        }
        return true;
    }

    public void CreateAllNodes()
    {
        DeleteNodes();

        //Create action container.
        nodes[NodeType.ACTION_CONTAINER] = new VBoxContainer();
        nodes[NodeType.ACTION_CONTAINER].SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        nodes[NodeType.ACTION_CONTAINER].SetAnchor(Side.Left, 0.6f);
        nodes[NodeType.ACTION_CONTAINER].SetAnchor(Side.Bottom, 0.65f);

        //Create the nodes to show stats.
        nodes[NodeType.STAT_CONTAINER] = new VBoxContainer();
        nodes[NodeType.STAT_CONTAINER].SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        nodes[NodeType.STAT_CONTAINER].SetAnchor(Side.Right, 0.4f);

        nodes[NodeType.NAME] = new Label();
        nodes[NodeType.DELAY] = new Label();

        nodes[NodeType.HEALTH] = new ProgressBar();
        nodes[NodeType.ENERGY] = new ProgressBar();


        //Add the container
        AddChild(nodes[NodeType.STAT_CONTAINER]);

        //Add the rest of the nodes to the container.
        foreach (Control node in nodes.Values)
        {
            //Skip the container.
            if(node == nodes[NodeType.STAT_CONTAINER]){continue;}

            //Set the sizing.
            node.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            node.SizeFlagsVertical = SizeFlags.ExpandFill;

            //Add it to the container.
            nodes[NodeType.STAT_CONTAINER].AddChild(node);
        }
    }

    public void DeleteNodes()
    {
        foreach (Node node in nodes.Values)
        {
            node.QueueFree();
        }
        nodes.Clear();
    }

    public void SetMob(Mob mob)
    {
        this.mob = mob;
        CreateAllNodes();
    }

    public void UpdateStatNodes()
    {
        #pragma warning disable CS8602 // Dereference of a possibly null reference.
        (nodes[NodeType.NAME] as Label).Text = mob.Identity.Name;
        (nodes[NodeType.DELAY] as Label).Text = mob.Stats.GetValue(StatSet.Name.DELAY).ToString();

        (nodes[NodeType.HEALTH] as ProgressBar).Value = mob.Stats.GetValue(StatSet.Name.HEALTH);
        (nodes[NodeType.HEALTH] as ProgressBar).MaxValue = mob.Stats.GetMax(StatSet.Name.HEALTH);

        (nodes[NodeType.ENERGY] as ProgressBar).Value = mob.Stats.GetValue(StatSet.Name.ENERGY);
        (nodes[NodeType.ENERGY] as ProgressBar).MaxValue = mob.Stats.GetMax(StatSet.Name.ENERGY);
        #pragma warning restore CS8602 // Dereference of a possibly null reference.

    }

    public void UpdateActionButtons()
    {
        foreach (Node node in nodes[NodeType.ACTION_CONTAINER].GetChildren())
        {
            node.QueueFree();
        }
        
        foreach (ChessLike.Entity.Action action in mob.actions)
        {
            ActionButton button = new(action);
            nodes[NodeType.ACTION_CONTAINER].AddChild(button);
            button.Pressed += () => ActionPressed?.Invoke(action);
        }
    }

    private partial class ActionButton : Button
    {
        ChessLike.Entity.Action action;

        public ActionButton(Action action)
        {
            this.action = action;
            Text = action.name;
        }
    }

}
