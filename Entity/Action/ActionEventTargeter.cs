using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World;
using ChessLike.WorldMap;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEventTargeter : Node3D
{

	public enum ETargetingType { AFFECTED, CAN_BE_TARGETED, TARGETED }
	const string TARGETING_NODE_GROUP = "ActionEventTargeterTARGETING_NODE_GROUP";
	const string AoE_NODE_GROUP = "ActionEventTargeterAoE_NODE_GROUP";
	const string META_KEY_MARKER_POSITION = "ActionEventTargeterMARKER_POSITION";
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

	protected UsageParameters? UsageParametersCurrent;

	public override void _Ready()
	{
		EventBus.CombatStateChanged += OnBattleStateChanged;
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	protected override void Dispose(bool disposing)
	{
		EventBus.CombatStateChanged -= OnBattleStateChanged;
		EventBus.CellInputReceived -= OnCellInputReceived;
		base.Dispose(disposing);
	}

	public void UpdateMarkers()
	{
		//SetMarkers(UsageParametersCurrent.PositionsTargeted, ETargetingType.TARGETED);
		SetMarkers(UsageParametersCurrent.PositionsAffected, ETargetingType.AFFECTED);
		SetMarkers(GetTargetableCells(UsageParametersCurrent), ETargetingType.CAN_BE_TARGETED);
	}

	public void SetMarkers(List<Vector3i> targets, ETargetingType targetingType)
	{
		ClearMarkers(targetingType);

		foreach (var gridPos in targets)
		{
			Node3D newNode = NodeTargeting;
			switch (targetingType)
			{
				case ETargetingType.AFFECTED:
					newNode = NodeAoE;
					break;

				case ETargetingType.CAN_BE_TARGETED:
					newNode = NodeTargeting;
					break;

				default: throw new Exception();
			}

			AddChild(newNode);
			Godot.Vector3 pos = CombatScene.GetGridNode().MapToGlobal(gridPos);
			newNode.SetMeta(META_KEY_MARKER_POSITION, gridPos.ToGVector3I());

			newNode.GlobalPosition = pos;
		}
	}

	protected List<Node3D> GetMarkers(ETargetingType targetingType)
	{
		string group;
		switch (targetingType)
		{
			case ETargetingType.CAN_BE_TARGETED:
				group = TARGETING_NODE_GROUP;
				break;

			case ETargetingType.AFFECTED:
				group = AoE_NODE_GROUP;
				break;

			case ETargetingType.TARGETED:
				group = "ActionEventTargeterUNUSED_GROUP";
				break;
			default: throw new Exception();
		}

		List<Node3D> nodes = new(GetTree().GetNodesInGroup(group).OfType<Node3D>());
		return new(nodes);
	}

	protected void ClearMarkers()
	{
		foreach (var item in Enum.GetValues<ETargetingType>())
		{
			ClearMarkers(item);
		}
	}

	protected void ClearMarkers(ETargetingType targetingType)
	{
		foreach (var item in GetMarkers(targetingType))
		{
			item.QueueFree();
		}
	}

	protected bool IsMarkerAtPosition(ETargetingType targetingType, Vector3i pos)
	{
		foreach (var node in GetMarkers(targetingType))
		{
			Vector3I nodePos = node.GetMeta(META_KEY_MARKER_POSITION).As<Vector3I>();

			if (nodePos == pos)
				return true;
		}
		return false;
	}

	public List<Vector3i> GetTargetableCells(UsageParameters parameters)
	{
		ActionEvent action = parameters.ActionRef;
		List<Vector3i> targets = action.GetTargetVectors(parameters ?? throw new Exception("Tried to get targeting range without UsageParameters"));
		return targets;
	}

	protected void ConfirmParameters()
	{
		UsageParametersCurrent.UpdateAffectedCells();
		UsageParametersCurrent.UpdateMobsTargeted();

		if (!UsageParametersCurrent.HasPositionsTargeted() || !UsageParametersCurrent.HasPositionsAffected())
			throw new Exception();	

		EventBus.TargetingParametersDone?.Invoke(UsageParametersCurrent);
		ClearMarkers();
	}

	protected bool IsCellTargeted(Vector3i position)
	{
		return UsageParametersCurrent.PositionsTargeted.Contains(position);
	}

	protected bool IsCellAffected(Vector3i position)
	{
		return UsageParametersCurrent.PositionsAffected.Contains(position);
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
		=> UsageParametersCurrent.PositionsTargeted.Count < parameters.ActionRef.GetMaxTargetingSelections();


	#region Event Handling
	private void OnBattleStateChanged(ECombatState state)
	{
		if (state == ECombatState.TARGETING)
		{
			UsageParametersCurrent = CombatScene.UsageParameters ?? throw new Exception();

			UsageParametersCurrent?.ResetTargets();

			//Set which cells can be targeted.
			UpdateMarkers();
		}

	}


	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		//Must be on targeting state.
		if (CombatScene.GetState() != ECombatState.TARGETING) return;

		//There must be UsageParameters
		if (UsageParametersCurrent is null) throw new Exception();

		/* 		ActionEvent action = UsageParametersCurrent.ActionRef;
				Mob owner = UsageParametersCurrent.OwnerRef;
				Grid grid = CombatScene.GetGrid(); */


		switch (input)
		{
			case ECellInput.PRIMARY:
				if (!CanSelectPosition(cellPos)) break;

				//Ran out of selections, proceed to confirm.
				if (!HasSelectionsLeft(UsageParametersCurrent))
				{
					//Must be a cell already set to be hit.
					if (IsCellAffected(cellPos))
					{
						ConfirmParameters();
					}
				}
				//There are selections left.
				else
				{
					//Before adding, make sure it isn't targeted already.
					if (IsCellTargeted(cellPos)) return;

					UsageParametersCurrent.PositionsTargeted.Add(cellPos);
					UsageParametersCurrent.UpdateAffectedCells();
					UpdateMarkers();
				}
				break;

			case ECellInput.SECONDARY:
				UsageParametersCurrent.ResetTargets();
				UpdateMarkers();
				break;

			default: break;
		}
	}
	
	#endregion
}
