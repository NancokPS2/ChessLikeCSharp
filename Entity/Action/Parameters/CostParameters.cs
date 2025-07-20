using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class CostParameters : Resource
{
	[Export]
	public int Action = 1;
	[Export]
	public int SubAction;
	[Export]
	public int Reaction;
}