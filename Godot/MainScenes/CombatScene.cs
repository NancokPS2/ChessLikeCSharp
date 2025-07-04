using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World.Encounter;
using Godot;

[GlobalClass]
public partial class CombatScene : Node3D
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	public static EncounterData EncounterData;

	[Export]
	private GridNode gridNode
	{
		set => GridNode = value;
		get => GridNode;
	}
	public static GridNode GridNode;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

	public void Setup(EncounterData encounterToLoad)
	{
		EncounterData = encounterToLoad;

		EventBus.EncounterLoading?.Invoke(encounterToLoad);

		foreach (var item in encounterToLoad.MobPlacement)
		{
			if (item.PresetMob is null) return;

			item.PresetMob.MobState = ChessLike.Entity.EMobState.COMBAT;
			//WIP This should be used automatically
			item.PresetMob.Move(new(item.Location));
		}

		//Everything must be loaded by now.
		EventBus.CombatStarted?.Invoke();
	}
}
