using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Godot;

namespace ChessLike.Shared;

public interface IGSceneAdapter
{
    public struct NodeDeclaration
    {
        public string NodeName;
        public Type? NodeType = null;
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
    public List<NodeDeclaration> NodesRequired {get;set;}
    //public Dictionary<string, NodeDeclaration> NodesRequiredDict {get;set;}

    public void RequiredNodeAdd(string node_name, Type? node_type = null, bool required = true, string group_source = "")
    {
        NodeDeclaration declaration = new(){NodeName = node_name, NodeType = node_type, Required = required};
        RequiredNodeAdd(declaration);
    }
    public void RequiredNodeAdd(NodeDeclaration declaration)
    {
        NodesRequired.Add(declaration);
    }

    public void RequiredNodeClear()
    {
        NodesRequired.Clear();
    }

    public bool RequiredNodeAllPresent(Node parent)
    {
        foreach (NodeDeclaration declaration in NodesRequired)
        {
            Node? found_node = RequiredNodeTryToGet(declaration, parent);
            if (found_node == null)
            {
                return false;
            }
        }

        return true;
    }

    public Node? RequiredNodeTryToGet(NodeDeclaration declaration, Node parent, bool ignore_group = false)
    {
        Node? output;

        if (declaration.GroupSource != "" && !ignore_group)
        {
            if (!parent.IsInsideTree()){throw new Exception("Node must be inside the tree");}
            output = parent.GetTree().GetFirstNodeInGroup(declaration.GroupSource);
        }else
        {
            output = parent.GetNodeOrNull(declaration.NodeName);
        }

        if (output != null && declaration.NodeType != null)
        {
            if (output.GetType() != declaration.NodeType)
            {
                throw new ArgumentException("Wrong node type " + output.GetType().ToString() + ". Expected " + declaration.NodeType.ToString());
            }
        }


        return output;
    }

    public void LoadScene(PackedScene packed, Node parent)
    {
        Node instance = packed.Instantiate();
        if (RequiredNodeAllPresent(instance))
        {
            parent.AddChild(instance);
        }else
        {
            throw new ArgumentException("Scene is not valid due to missing nodes.");
        }
    }
}
