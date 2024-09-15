using ChessLike.Extension;
using ChessLike.Shared;
using Godot;
using System;

namespace Godot.Menu;

public partial class Pause : Control
{
    public static List<NodeRequirement> NodesRequired { get; set; } = new()
	{
		new ("RESUME", typeof(Button)),
		new ("OPTIONS", typeof(Button)),
		new ("QUIT", typeof(Button)),
	};
    public const string SCENE_PATH = "res://assets/PackedScene/Pause.tscn";
}
