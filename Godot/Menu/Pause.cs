using ChessLike.Shared;
using Godot;
using System;

namespace Godot.Menu;

public partial class Pause : Control, IGSceneAdapter
{
    public List<IGSceneAdapter.NodeDeclaration> NodesRequired { get; set; } = new()
	{
		new ("RESUME", typeof(Button)),
		new ("OPTIONS", typeof(Button)),
		new ("QUIT", typeof(Button)),
	};
    public Node? SceneTopNode { get; set; }
    public string ScenePath { get; set; } = "res://assets/PackedScene/Pause.tscn";

    public override void _Ready()
	{
	}

    public T? RequiredNodeTryToGet<T>(IGSceneAdapter.NodeDeclaration declaration, bool ignore_group = false)
    {
        return ((IGSceneAdapter)this).RequiredNodeTryToGet<T>(declaration, ignore_group);
    }
}
