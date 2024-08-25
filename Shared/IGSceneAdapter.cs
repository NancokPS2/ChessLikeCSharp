using static ChessLike.Shared.IGSceneAdapterExtension;
using Godot;
using static ChessLike.Shared.IGSceneAdapter;

namespace ChessLike.Shared;

/// <summary>
/// Interface that helps in running a node that relies on other nodes.
/// It is not recommended to include scripts in any of the nodes in the given scenes.
/// </summary>
public interface IGSceneAdapter
{
    public List<NodeDeclaration> NodesRequired {get;set;}
    public Node? SceneTopNode {get;set;}
    public string ScenePath {get;set;}

    public static void Setup(IGSceneAdapter target, Node scene_parent, bool strict = true)
    {
        PackedScene scene = GD.Load<PackedScene>(target.ScenePath);
        Node instantiated = scene.Instantiate();
        scene_parent.AddChild(instantiated);
        target.SceneTopNode = instantiated;
        if (strict && !IGSceneAdapterExtension.RequiredNodeAllPresent(target))
        {
            throw new Exception("Could not find some required nodes. >" + target.SceneTopNode.GetChildren().ToString());
        } 
    }

    public static void RemoveScene(IGSceneAdapter target)
    {
        target.SceneTopNode.GetParent().RemoveChild(target.SceneTopNode);
    }

    public struct NodeDeclaration
    {
        public string NodeName;
        public Type NodeType = typeof(Node);
        public bool Required = true;
        public string GroupSource = "";

        public NodeDeclaration(string NodeName)
        {
            this.NodeName = NodeName;
        }

        public NodeDeclaration(string NodeName, Type NodeType)
        {
            this.NodeName = NodeName;
            this.NodeType = NodeType;
        }

        public NodeDeclaration(string NodeName, Type NodeType, bool Required, string GroupSource) : this(NodeName, NodeType)
        {
            this.Required = Required;
            this.GroupSource = GroupSource;
        }


        public static List<NodeDeclaration> GenerateFromStrings(string[] strings)
        {
            List<NodeDeclaration> output = new();

            foreach (string name in strings)
            {
                output.Add(new(name));
            }

            return output;
        }
        public static List<NodeDeclaration> GenerateFromDictNameType(Dictionary<string, Type> dictionary)
        {
            List<NodeDeclaration> output = new();

            foreach (string name in dictionary.Keys)
            {
                Type type = dictionary[name];
                output.Add(new(name, type));
            }

            return output;
        }

    }
}

public static class IGSceneAdapterExtension
{


    public static void RequiredNodeAdd(this IGSceneAdapter @this, string node_name, Type node_type, bool required = true, string group_source = "")
    {
        NodeDeclaration declaration = new(){NodeName = node_name, NodeType = node_type, Required = required};
        IGSceneAdapterExtension.RequiredNodeAdd(@this, declaration);
    }

    public static void RequiredNodeAdd(this IGSceneAdapter @this, NodeDeclaration declaration)
    {
        @this.NodesRequired.Add(declaration);
    }

    public static void RequiredNodeClear(this IGSceneAdapter @this)
    {
        @this.NodesRequired.Clear();
    }

    public static bool RequiredNodeAllPresent(this IGSceneAdapter @this)
    {
        foreach (NodeDeclaration declaration in @this.NodesRequired)
        {
            Node? found_node = RequiredNodeTryToGet<Node>(@this, declaration);
            if (found_node == null)
            {
                return false;
            }
        }

        return true;
    }

    public static List<Node> RequiredNodeGetAll(this IGSceneAdapter @this)
    {
        List<Node> output = new();
        foreach (NodeDeclaration declaration in @this.NodesRequired)
        {
            Node? found_node = RequiredNodeTryToGet<Node>(@this, declaration);
            if (found_node != null)
            {
                output.Add(found_node);
            }else{
                throw new Exception("Make sure the nodes exist before trying to get all of them.");
            }
        }
        return output;
    }

    public static T? RequiredNodeTryToGet<T>(this IGSceneAdapter @this, NodeDeclaration declaration, bool ignore_group = false)
    {
        T? output = default;

        if (declaration.GroupSource != "" && !ignore_group)
        {
            if (!@this.SceneTopNode.IsInsideTree()){throw new Exception("Node must be inside the tree");}
            Node node_in_tree = @this.SceneTopNode.GetTree().GetFirstNodeInGroup(declaration.GroupSource);
            if (node_in_tree is T typed)
            {
                output = typed;
            }
        }else
        {
            Node node_in_tree = @this.SceneTopNode.GetNodeOrNull(declaration.NodeName);
            if (node_in_tree is T typed)
            {
                output = typed;
            }
        }
        if (output == null)
        {
            Node node_in_tree = @this.SceneTopNode.FindChild(declaration.NodeName, true, false);;
            if (node_in_tree is T typed)
            {
                output = typed;
            } else
            {
                throw new Exception("Cannot find the node! >" + @this.SceneTopNode.GetChildren().ToString());
            }
        }

        return output;
    }
}