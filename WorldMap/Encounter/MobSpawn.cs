using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using Godot;

namespace ChessLike.WorldMap;

[GlobalClass]
public partial class MobSpawn : Resource
{

	[Export]
	public Mob? Mob = null;

	[Export]
	public Godot.Collections.Array<MobTemplateBase> TemplatesBase = new();

	[Export]
	public Godot.Collections.Array<MobTemplateRace> TemplatesRace = new();

	[Export]
	public Godot.Collections.Array<MobTemplateIdentity> TemplatesIdentity = new();

	[Export]
	public Godot.Collections.Array<MobTemplateJob> TemplatesJob = new();


	public Mob GetNewMob()
	{
		Mob mob = new();
		if (TemplatesBase.Count != 0) mob.TemplateSet(TemplatesBase.PickRandom());
		if (TemplatesRace.Count != 0) mob.TemplateSet(TemplatesRace.PickRandom());
		if (TemplatesIdentity.Count != 0) mob.TemplateSet(TemplatesIdentity.PickRandom());
		if (TemplatesJob.Count != 0) mob.TemplateSet(TemplatesJob.PickRandom());
		mob.TemplateUpdate(true, true);

		return mob;
	}
}
