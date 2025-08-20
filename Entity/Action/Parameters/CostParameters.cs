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
	[Export]
	public int Move;

	public override string ToString()
	{
		return $"{(Action != 0 ? Action.ToString()+"\n" : "")}{(SubAction != 0 ? SubAction.ToString()+"\n" : "")}{(Reaction != 0 ? Reaction.ToString()+"\n" : "")}{(Move != 0 ? Move.ToString()+"\n" : "")}";

	}

}