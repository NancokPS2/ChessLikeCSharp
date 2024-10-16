using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class JobResource : Godot.Resource
{
	[Export]
	public EJob Identifier = EJob.DEFAULT;
	[Export]
	public MobStatSetResource Stats = new();
	[Export]
	public Godot.Collections.Array<AbilityResource> Actions = new();
}
