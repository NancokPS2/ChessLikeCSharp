using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEventTargeter : Node3D
{
	public enum ETargetingType { AOE, TARGETING, SELECTED }
	const string TARGETING_NODE_GROUP = "ActionEventTargeterTARGETING_NODE_GROUP";
	const string AoE_NODE_GROUP = "ActionEventTargeterAoE_NODE_GROUP";
	[Export]
	protected PackedScene? SceneTargeting;
	public Node3D NodeTargeting
	{
		get
		{
			Node3D node = SceneTargeting?.Instantiate<Node3D>() ?? throw new Exception();
			node.AddToGroup(TARGETING_NODE_GROUP);
			return node;
		}
	}

	[Export]
	protected PackedScene? SceneAoE;
	public Node3D NodeAoE
	{
		get
		{

			var node = SceneAoE?.Instantiate<Node3D>() ?? throw new Exception();
			node.AddToGroup(AoE_NODE_GROUP);
			return node;
		}
	}

	private List<Vector3i> PositionsTargeting = new();
	private List<Vector3i> PositionsSelected = new();
	private List<List<Vector3i>> PositionsAoE = new();

	protected UsageParameters? UsageParametersCurrent;

	public ActionEventTargeter()
	{
		EventBus.BattleStateChanged += OnBattleStateChanged;
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	public void SetMarkers(List<Vector3i> targets, ETargetingType targetingType)
	{
		ClearMarkers(targetingType);

		foreach (var item in targets)
		{
			Node3D newNode = NodeTargeting;
			switch (targetingType)
			{
				case ETargetingType.AOE:
					newNode = NodeAoE;
					break;

				case ETargetingType.TARGETING:
					newNode = NodeTargeting;
					break;

				default: throw new Exception();
			}

			AddChild(newNode);
			Godot.Vector3 pos = CombatScene.GetGridNode().MapToGlobal(item);

			newNode.GlobalPosition = pos;
		}
	}

	protected void ClearMarkers(ETargetingType targetingType)
	{
		string group;
		switch (targetingType)
		{
			case ETargetingType.TARGETING:
				group = TARGETING_NODE_GROUP;
				break;

			case ETargetingType.AOE:
				group = AoE_NODE_GROUP;
				break;

			default: throw new Exception();
		}

		Godot.Collections.Array<Node> nodes = GetTree().GetNodesInGroup(group);
		foreach (var item in nodes)
		{
			item.QueueFree();
		}
	}

	protected void SetTargetingCells(UsageParameters parameters)
	{
		ActionEvent action = parameters.ActionRef;
		List<Vector3i> targets = action.GetTargetVectors(UsageParametersCurrent ?? throw new Exception("Tried to get targeting range without UsageParameters"));

		//Display the positions
		SetMarkers(targets, ETargetingType.TARGETING);
		PositionsTargeting = targets;
	}

	protected void UpdateAoECells(UsageParameters parameters)
	{
		ActionEvent action = parameters.ActionRef;
		List<List<Vector3i>> targetClusters = action.GetAoEVectors(
			UsageParametersCurrent ?? throw new Exception("Tried to get targeting range without UsageParameters"),
	PositionsSelected
			);

		List<Vector3i> markerPositions = new();
		foreach (var item in targetClusters)
		{
			markerPositions.AddRange(item);
		}
		SetMarkers(markerPositions, ETargetingType.AOE);
		PositionsAoE = targetClusters;
	}

	public void AddSelectedCell(Vector3i position)
	{
		PositionsSelected.Add(position);
	}

	protected void ClearTargetedCells(ETargetingType targetingType)
	{
		switch (targetingType)
		{
			case ETargetingType.TARGETING:
				PositionsTargeting.Clear();
				ClearMarkers(ETargetingType.TARGETING);
				break;

			case ETargetingType.AOE:
				PositionsAoE.Clear();
				ClearMarkers(ETargetingType.AOE);
				break;

			case ETargetingType.SELECTED:
				PositionsSelected.Clear();
				break;

			default: throw new Exception();
		}
	}

	protected void ConfirmSelection()
	{
		//Get the action and ensure there parameters are valid.
		ActionEvent action = UsageParametersCurrent?.ActionRef ?? throw new Exception();

		List<Vector3i> positionsAffected = new();
		foreach (var item in PositionsAoE) positionsAffected.AddRange(item);

		//Store all positions affected
		foreach (var item in positionsAffected)
		{
			UsageParametersCurrent.PositionsTargeted.Add(item, false);
		}

		//All mobs that can be targeted, are added to the usage parameters.
		foreach (var mob in CombatScene.GetMobsInCombat())
		{
			if (!positionsAffected.Contains(mob.GetPosition())) continue;
			if (!action.IsMobValid(mob)) continue;

			UsageParametersCurrent.MobsTargeted.Add(mob);
		}

		EventBus.TargetingParametersDone?.Invoke(UsageParametersCurrent);
		Reset();
	}

	public bool IsCellTargeted(Vector3i position, ETargetingType targetingType)
	{
		switch (targetingType)
		{
			case ETargetingType.TARGETING:
				return PositionsTargeting.Contains(position);

			case ETargetingType.AOE:
				foreach (var item in PositionsAoE)
				{
					if (item.Contains(position))
					{
						return true;
					}
				}
				return false;

			case ETargetingType.SELECTED:
				return PositionsSelected.Contains(position);

			default: throw new Exception();
		}
	}

	protected Mob? GetMobAtPosition(Vector3i pos)
		=> CombatScene.GetMobsInCombat().Find(x => x.GetPosition() == pos);

	protected bool CanSelectPosition(Vector3i pos)
	{
		Mob? mob = GetMobAtPosition(pos);
		bool hasMob = mob is not null;
		ActionEvent action = UsageParametersCurrent?.ActionRef ?? throw new Exception();

		//If it does NOT have a mob, but can't target empty spots, fail.
		if (!hasMob && !action.TargetParams.CanTargetEmpty) return false;

		//If it has a mob, but can't target spots with mobs, fail.
		if (hasMob && !action.TargetParams.CanTargetWithMob) return false;

		return true;
	}

	protected bool HasSelectionsLeft(UsageParameters parameters)
		=> PositionsSelected.Count < parameters.ActionRef.GetMaxTargetingSelections();

	private void Reset()
	{
		ClearTargetedCells(ETargetingType.AOE);
		ClearTargetedCells(ETargetingType.TARGETING);
		ClearTargetedCells(ETargetingType.SELECTED);
		UsageParametersCurrent = null;
	}

	#region Event Connection
	private void OnBattleStateChanged(EBattleState state)
	{
		Reset();

		if (state == EBattleState.TARGETING)
		{
			UsageParametersCurrent = CombatScene.UsageParameters ?? throw new Exception();

			//Set which cells can be targeted.
			SetTargetingCells(UsageParametersCurrent);
		}

	}


	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		//Must be on targeting state.
		if (CombatScene.GetState() != EBattleState.TARGETING) return;

		//There must be UsageParameters
		if (UsageParametersCurrent is null) throw new Exception();

		ActionEvent action = UsageParametersCurrent.ActionRef;
		Mob owner = UsageParametersCurrent.OwnerRef;
		Grid grid = CombatScene.GetGrid();


		switch (input)
		{
			case ECellInput.PRIMARY:
				if (!CanSelectPosition(cellPos)) break;

				//Ran out of selections, proceed to confirm.
				if (!HasSelectionsLeft(UsageParametersCurrent))
				{
					//Must be a cell already set to be hit.
					if (!IsCellTargeted(cellPos, ETargetingType.AOE)) break;

					UpdateAoECells(UsageParametersCurrent);
					ConfirmSelection();
				}
				//There are selections left.
				else
				{
					//Before adding, make sure it isn't targeted already.
					if (!IsCellTargeted(cellPos, ETargetingType.TARGETING)) return;

					AddSelectedCell(cellPos);
					UpdateAoECells(UsageParametersCurrent);
				}
				break;

			case ECellInput.SECONDARY:
				ClearTargetedCells(ETargetingType.SELECTED);
				ClearTargetedCells(ETargetingType.AOE);
				break;

			default: break;
		}
	}
	
	#endregion
}
