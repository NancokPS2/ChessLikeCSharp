using ChessLike.Entity;

namespace Godot.Display.Interface;

public partial class MobUI : Control
{
    public enum NodeType
    {
        CONTAINER, //VBoxContainer
        NAME, DELAY, //Label
        HEALTH, ENERGY, //ProgressBar
    }
    private Dictionary<NodeType, Control> nodes = new();
    private Mob mob;

    public MobUI(Mob mob)
    {
        this.mob = mob;
        CreateNodes();
    }

    public void CreateNodes()
    {
        DeleteNodes();

        //Create the nodes.
        nodes[NodeType.CONTAINER] = new VBoxContainer();
        nodes[NodeType.CONTAINER].SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        nodes[NodeType.NAME] = new Label();
        nodes[NodeType.DELAY] = new Label();

        nodes[NodeType.HEALTH] = new ProgressBar();
        nodes[NodeType.ENERGY] = new ProgressBar();


        //Add the container
        AddChild(nodes[NodeType.CONTAINER]);

        //Add the rest of the nodes to the container.
        foreach (Control node in nodes.Values)
        {
            //Skip the container.
            if(node == nodes[NodeType.CONTAINER]){continue;}

            //Set the sizing.
            node.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            node.SizeFlagsVertical = SizeFlags.ExpandFill;

            //Add it to the container.
            nodes[NodeType.CONTAINER].AddChild(node);
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
        CreateNodes();
    }

    public void UpdateNodes()
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

}
