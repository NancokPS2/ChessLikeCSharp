using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public static class NodeExtension
{
	private const string META_KEY_ORIGINAL_PARENT = "NodeRemovedOriginalParent";

	public static void RemoveSelf(this Node @this, bool recordParent = false)
    {
        Node parent = @this.GetParent();
        if (@parent is null){GD.PushWarning("Cannot remove self, it is an orphan."); return;}

		if (recordParent) RecordOriginalParent(@this);

        parent.RemoveChild(@this);
    }

	public static void RecordOriginalParent(this Node @this)
		=> @this.SetMeta(META_KEY_ORIGINAL_PARENT, @this.GetParent());

	public static void AddSelf(this Node @this)
	{
		if (!@this.HasMeta(META_KEY_ORIGINAL_PARENT))
			throw new Exception("No original parent remembered.");
		if (!GodotObject.IsInstanceValid(@this.GetMeta(META_KEY_ORIGINAL_PARENT).As<Node>()))
			throw new Exception("The parent is no longer valid.");

		@this.GetMeta(META_KEY_ORIGINAL_PARENT).As<Node>().AddChild(@this);
	}

    public static void ToggleFromScene(this Node @this)
    {
        if (@this.IsInsideTree()) @this.RemoveSelf();
        else if (!@this.IsInsideTree()) @this.AddSelf();
    }

    public static void FreeChildren(this Node @this)
    {
        foreach (var item in @this.GetChildren())
        {
            item.QueueFree();
        }
    }

    /// <summary>
    /// WARNING: Using this can lead to memory leaks.
    /// </summary>
    /// <param name="this"></param>
    public static void RemoveChildren(this Node @this)
    {
        foreach (var item in @this.GetChildren())
        {
            @this.RemoveChild(item);
        }
    }

    public static ICollection<TNodeType> GetChildren<TNodeType>(this Node @this) where TNodeType : Node
    {
        List<TNodeType> output = new();
        foreach (var item in @this.GetChildren())
        {
            if (item is TNodeType typed)
            {
                output.Add(typed);
            }
        }
        return output;
    }

	public static List<Node> GetChildrenRecursive(this Node node)
	{
		List<Node> output = new();

		foreach (var child in node.GetChildren())
		{
			if (child.GetChildCount() > 0)
			{
				output.Add(child);
				output.AddRange(child.GetChildrenRecursive());
			}
			else
			{
				output.Add(child);
			}
		}

		return output;
	}

    public static TNodeType? GetChild<TNodeType>(this Node @this) where TNodeType : Node
	{
		List<TNodeType> output = new();
		foreach (var item in @this.GetChildren())
		{
			if (item is TNodeType typed)
			{
				return typed;
			}
		}
		return null;
	}

    public static void ParentToRoot(this Node @this, Node node_in_tree)
    {
        if (!node_in_tree.IsInsideTree()){throw new ArgumentException("Node must be inside the tree to be useful.");}
        if (@this.IsInsideTree()){@this.RemoveSelf();}

        node_in_tree.GetTree().Root.AddChild(@this);
    }

    public static bool IsAloneInGroup(this Node node, string group)
    {
        var group_arr = node.GetTree().GetNodesInGroup(group);
        bool has_one = group_arr.Count == 1;
        bool includes_this = group_arr.Contains(node);

        return has_one && includes_this;
    }    

    public static TButton AddListButton<TButton>(this Control @this, bool expanded = true) where TButton : BaseButton, new()
    {
        TButton button = new();
        if (expanded) {button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill; button.AnchorRight = 1.0f;}
        @this.AddChild(button);
        return button;
    }
}

public struct NodeRequirement
{
    public string NodeName;
    public Type NodeType = typeof(Node);
    public bool Required = true;
    public string GroupSource = "";

    public NodeRequirement(string NodeName)
    {
        this.NodeName = NodeName;
    }

    public NodeRequirement(string NodeName, Type NodeType)
    {
        this.NodeName = NodeName;
        this.NodeType = NodeType;
    }

    public NodeRequirement(string NodeName, Type NodeType, bool Required, string GroupSource) : this(NodeName, NodeType)
    {
        this.Required = Required;
        this.GroupSource = GroupSource;
    }

}