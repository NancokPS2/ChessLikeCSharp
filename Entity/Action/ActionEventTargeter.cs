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

	public ActionEventTargeter()
	{
		EventBus.BattleStateChanged += OnBattleStateChanged;
		EventBus.EncounterLoading += OnEncounterLoading;
		EventBus.CellInputReceived += OnCellInputReceived;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void TargetingRangeDisplay(ActionEvent action)
	{
	}



	#region Event Connection
	private void OnBattleStateChanged(EBattleState state)
	{
		if (state == EBattleState.TARGETING)
		{
			if (CombatScene.UsageParameters is null) throw new Exception();

			ActionEvent action = CombatScene.UsageParameters.ActionRef;
			List<Vector3i> targets = action.GetTargetVectors(CombatScene.UsageParameters);

			//This is not working
			foreach (var item in targets)
			{
				Node3D newNode = NodeTargeting;
				AddChild(newNode);
				
				Godot.Vector3 pos = CombatScene.GetGridNode().MapToGlobal(item);

				newNode.GlobalPosition = pos;
			}
		}
		else
		{
			ClearTargeting();
			ClearAoE();
		}
	}

	public void ClearAoE()
	{
		Godot.Collections.Array<Node> nodes = GetTree().GetNodesInGroup(AoE_NODE_GROUP);
		foreach (var item in nodes)
		{
			item.QueueFree();
		}
	}

	public void ClearTargeting()
	{
		Godot.Collections.Array<Node> nodes = GetTree().GetNodesInGroup(TARGETING_NODE_GROUP);
		foreach (var item in nodes)
		{
			item.QueueFree();
		}
	}

	private void OnCellInputReceived(Vector3i cellPos, GridCell cell, ECellInput input)
	{
		if (CombatScene.GetState() != EBattleState.TARGETING) return;
		if (CombatScene.UsageParameters is null) throw new Exception();

		ActionEvent action = CombatScene.UsageParameters.ActionRef;
		Mob owner = CombatScene.UsageParameters.OwnerRef;
		Grid grid = CombatScene.GetGrid();

		switch (input)
		{
			case ECellInput.PRIMARY:
				break;

			case ECellInput.SECONDARY:
				break;

			default:  break;
		}
	}
	
	private void OnEncounterLoading(EncounterData obj)
	{
	}
	#endregion
}
