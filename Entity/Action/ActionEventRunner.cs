using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Turn;
using ChessLike.World;
using ChessLike.WorldMap;
using Godot;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEventRunner : Node3D
{
	public delegate void ActionQueue(ActionEvent action, UsageParameters parameters);
	public delegate void Delegate();

	#region Queue
	protected bool ReadyToStartRun;
	private List<UsageParameters> Queue = new();

	public override void _Ready()
	{
		base._Ready();
		EventBus.CombatStateChanged += OnBattleStateChanged;
		EventBus.ActionQueueRequested += OnActionQueueRequested;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (ReadyToStartRun)
		{
			//If the CombatScene has defined any parameters, add them first.
			if (CombatScene.UsageParameters is not null)
				QueueAdd(CombatScene.UsageParameters);
				
			QueueRun();
		}
	}


	public void QueueAdd(UsageParameters parameters)
	{
		//Actually try to queue it.
		int index = Queue.Count;
		QueueInsert(parameters, index);
	}

	public void QueueAddBefore(UsageParameters parametersToAdd, UsageParameters parametersToDisplace)
	{
		int index = Queue.IndexOf(parametersToDisplace);
		Debug.Assert(index >= 0, "Index is invalid.");
		QueueInsert(parametersToAdd, index);
		Debug.Assert(Queue[index + 1] == parametersToDisplace, "The displaced parameter should end up AFTER the chosen index.");

		Console.WriteLine($"{parametersToAdd} added before {parametersToDisplace}");
	}

	private void QueueInsert(UsageParameters parameters, int index)
	{
		//Make sure it is valid first.
		if (!parameters.IsValid()) { throw new Exception("Invalid parameters."); }

		//Warn other actions about this one, so they can queue first.
		EventBus.ActionPreQueued?.Invoke(parameters);

		Queue.Insert(
			index,
			parameters
			);

		EventBus.ActionQueued?.Invoke(parameters);
	}

	public bool QueueIsEmpty() => Queue.Count == 0;

	private void QueueClear()
	{
		Queue.Clear();
	}
	#endregion

	#region Run Logic
	// RUN LOGIC

	public void QueueRun()
	{
		ReadyToStartRun = false;

		if (Queue.Count == 0) { throw new Exception("Nothing to run."); }

		for (int queueIndex = 0; queueIndex < Queue.Count; queueIndex++)
		{
			//Select the action to run.
			UsageParameters parametersToUse = Queue[queueIndex];

			//Use it
			if (!parametersToUse.Cancelled)
			{
				EventBus.ActionPreUsed?.Invoke(parametersToUse);
				parametersToUse.ActionRef.Use(parametersToUse);
				MsgLog.LogGameMsg(parametersToUse.ActionRef.GetUseText(parametersToUse));
			}
		}
		EventBus.ActionEventQueueFinished?.Invoke(Queue);

		Console.WriteLine($"Ran queued actions: {Queue.ToStringList()}");

		QueueClear();
	}
	#endregion

	#region Event Handling
	private void OnBattleStateChanged(ECombatState state)
	{
		if (state == ECombatState.ACTION_RUNNING)
		{
			ReadyToStartRun = true;
		}
	}

	private void OnActionQueueRequested(UsageParameters parameters)
	{
		QueueAdd(parameters);
		ReadyToStartRun = true;
	}
    #endregion
}
