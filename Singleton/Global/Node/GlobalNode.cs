using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Merely initializes the Global
/// </summary>
partial class GlobalNode : Node
{
	protected static bool instanced;
	public override void _Ready()
	{
		base._Ready();
		if (instanced) throw new Exception("Initialized twice!?");
		Global.ConnectToWindow(GetWindow());
		instanced = true;
	}
}
