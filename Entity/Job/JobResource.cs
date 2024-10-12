using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public Godot.Collections.Array<ActionResource> Actions = new();
}
