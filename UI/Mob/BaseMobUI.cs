using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

public partial class BaseMobUI : Control
{
	public override void _Ready()
	{
		base._Ready();
		EventBus.MobSelected += OnMobSelected;
	}

	protected virtual void Update(Mob mob){}

	protected void OnMobSelected(Mob obj)
	{
		Update(obj);
	}
}
