using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using ChessLike.World;
using Godot;
using Sprache;

namespace ChessLike.Entity.Action;

/// <summary>
/// Variables for Effect usage. Must be filled in the order of the variables.
/// </summary>
public partial class UsageParameters
{
	public Mob OwnerRef;
	public Grid GridRef;
	public ActionEvent ActionRef;
	/// <summary>
	/// Positions that where selected during the targeting of this action
	/// </summary>
	/// <returns></returns>
	public UniqueList<Vector3i> PositionsTargeted = new();

	public UniqueList<Vector3i> PositionsAffected = new();
	/// <summary>
	/// Mobs found in the targeted locations or caught in the AoE and filtered afterwards to be deemed as valid to affect.
	/// </summary>
	/// <returns></returns>
	public UniqueList<Mob> MobsTargeted = new();

	public bool Cancelled = false; 

	public int Priority = 0;

	public UsageParameters(Mob owner, Grid grid, ActionEvent action_reference)
	{
		this.OwnerRef = owner;
		this.GridRef = grid;
		this.ActionRef = action_reference;
	}

	public UsageParameters(UsageParameters parameters) : this(parameters.OwnerRef, parameters.GridRef, parameters.ActionRef)
	{
		PositionsTargeted = parameters.PositionsTargeted;
		MobsTargeted = parameters.MobsTargeted;
		Priority = parameters.Priority;
	}

	public void UpdateAffectedCells()
	{
		if (!IsValid())
			throw new Exception();
		if (!HasPositionsTargeted())
			throw new Exception();

		//Get the positions from the action.
		List<List<Vector3i>> targetClusters = ActionRef.GetAffectedVectors(this);

		foreach (var cluster in targetClusters)
		{
			foreach (var item in cluster)
			{
				PositionsAffected.Add(item, false);
			}
		}
	}

	public void UpdateMobsTargeted()
	{
		if (PositionsAffected.IsEmpty())
			throw new Exception("Cannot update targeted mobs without any positions.");

		MobsTargeted.Clear();
		foreach (var mob in CombatScene.GetMobsInCombat())
		{
			if (!PositionsAffected.Contains(mob.GetPosition())) continue;
			if (!ActionRef.IsMobValidForAoE(mob)) continue;

			MobsTargeted.Add(mob);
		}
	}

	public void ResetTargets()
	{
		PositionsTargeted.Clear();
		PositionsAffected.Clear();
		MobsTargeted.Clear();
	}

	public bool IsValid()
		=> OwnerRef is not null && GridRef is not null && ActionRef is not null;

	public bool HasPositionsTargeted()
		=> PositionsTargeted.Count > 0;

	public bool HasPositionsAffected()
		=> PositionsAffected.Count > 0;

	public bool HasMobsTargeted()
		=> MobsTargeted.Count > 0;
}

