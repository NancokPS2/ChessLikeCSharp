using ChessLike.Shared;
using Godot;
using System;

public partial class Pause : Control, IGSceneAdapter
{
    public List<IGSceneAdapter.NodeDeclaration> NodesRequired { get; set; } = new()
	{
		new ("RESUME", typeof(Button)),
		new ("OPTIONS", typeof(Button)),
		new ("QUIT", typeof(Button)),
	};

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if (!((IGSceneAdapter)this).RequiredNodeAllPresent(this))
		{
			GD.PushError("Missing nodes.");
		}
		
		((IGSceneAdapter)this).RequiredNodeTryToGet<Button>(
			NodesRequired[0],
			this
		);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
