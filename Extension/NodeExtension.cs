using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public static class NodeExtension
{
    public static void RemoveSelf(this Node @this)
    {
        Node parent = @this.GetParent();
        if (@parent is null){GD.PushWarning("Cannot remove self, it is an orphan."); return;}

        parent.RemoveChild(@this);
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

    public static Node AddSceneWithDeclarations(this Node @this, string scene_path, List<NodeRequirement> required, bool strict = true)
    {
        PackedScene scene = GD.Load<PackedScene>(scene_path);
        Node instantiated = scene.Instantiate();
        @this.AddChild(instantiated);
        if (strict && !instantiated.HasAllRequired(required))
        {
            throw new Exception("Could not find some required nodes. >" + @this.GetChildren().ToString());
        } 
        return instantiated;
    }

    public static NodeType AddSceneWithDeclarations<NodeType>(this Node @this, string scene_path, List<NodeRequirement> required, bool strict = true) where NodeType : Node
    {
        Node output = AddSceneWithDeclarations(@this, scene_path, required, strict);
        if (output is NodeType correct)
        {
            return correct;
        }
        else
        {
            throw new Exception("Wrong node type.");
        }
    }


    public static T? GetNodeFromRequirement<T>(this Node @this, NodeRequirement declaration, bool ignore_group = false)
    {
        T? output = default;

        if(declaration.NodeType is T){throw new Exception("The node in this declaration is of a different type.");}

        if (declaration.GroupSource != "" && !ignore_group)
        {
            if (!@this.IsInsideTree()){throw new Exception("Node must be inside the tree");}
            Node node_in_tree = @this.GetTree().GetFirstNodeInGroup(declaration.GroupSource);
            if (node_in_tree is T typed)
            {
                output = typed;
            }
        }
        else
        {
            Node node_in_tree = @this.GetNodeOrNull(declaration.NodeName);
            if (node_in_tree is T typed)
            {
                output = typed;
            }
        }
        if (output == null)
        {
            Node node_in_tree = @this.FindChild(declaration.NodeName, true, false);;
            if (node_in_tree is T typed)
            {
                output = typed;
            } 
            else
            {
                throw new Exception("Cannot find the node! >" + @this.GetChildren().ToString());
            }
        }

        return output;
    }

    public static string GetPathOfNeighbourScene(this Node @this)
    {
        if (@this is Node node)
        {
            Script script = (Script)node.GetScript();
            string scene_path = script.ResourcePath;
            scene_path = scene_path.Replace(".gd", ".tscn");
            return scene_path;
        }

        return "";
    }

    public static PackedScene GetPackedFromScriptPath(this Node @this)
    {
        return GD.Load<PackedScene>(@this.GetPathOfNeighbourScene());
    }

    public static bool HasAllRequired(this Node @this, List<NodeRequirement> nodes_required)
    {
        foreach (NodeRequirement declaration in nodes_required)
        {
            Node? found_node = GetNodeFromRequirement<Node>(@this, declaration);
            if (found_node == null)
            {
                return false;
            }
        }

        return true;
    }

    public static void HasAllRequiredAssert(this Node @this, List<NodeRequirement> nodes_required)
    {
        Debug.Assert(HasAllRequired(@this, nodes_required));
    }

    public static List<Node> TryGetNodesRequired(this Node @this, List<NodeRequirement> nodes_required)
    {
        List<Node> output = new();
        foreach (NodeRequirement declaration in nodes_required)
        {
            Node? found_node = GetNodeFromRequirement<Node>(@this, declaration);
            if (found_node != null)
            {
                output.Add(found_node);
            }else{
                throw new Exception("Make sure the nodes exist before trying to get all of them.");
            }
        }
        return output;
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