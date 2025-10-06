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
	public static event EventBus.ObjectChange<ActionEventRunner>? QueueEmptied; 

	public static ActionEventRunner Instance = null!;

	#region Queue
	protected bool QueueProcessing;
	private List<UsageParameters> Queue = new();

	public override void _Ready()
	{
		base._Ready();
		Instance = this;
		EventBus.CombatStateChanged += OnBattleStateChanged;
		EventBus.ActionQueueRequested += OnActionQueueRequested;
	}

	public static void StartQueueProcessing(bool enabled)
		=> Instance.QueueProcessing = enabled;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (!QueueProcessing)
			return;

		//If the CombatScene has defined any parameters, add them first.
		if (CombatScene.UsageParameters is not null)
			QueueAdd(CombatScene.UsageParameters);

		QueueRunNext();

	}

	public void QueueProcess()
	{
		if (!QueueProcessing)
			return;

		UsageParameters? currentParams = Queue.FirstOrDefault();

		//Nothing to process, stop processing.
		if (currentParams is null)
		{
			QueueProcessing = false;
			return;
		}

		//Discard if it was cancelled.
		if (currentParams.Cancelled)
		{
			Queue.Remove(currentParams);
			return;
		}

		//Discard if it was used.
		if (currentParams.Used && !currentParams.Animating)
		{
			Queue.Remove(currentParams);
			return;
		}

		//Make sure it is ready for use.
		bool ownerNull = currentParams.OwnerRef is null;
		bool gridNull = currentParams.GridRef is null;
		bool positionsEmpty = currentParams.PositionsTargeted.IsEmpty();
		bool positionsAndMobsNotAffected = currentParams.PositionsAffected.IsEmpty() && currentParams.MobsTargeted.IsEmpty();
		if (ownerNull || gridNull || positionsEmpty || positionsAndMobsNotAffected)
			return;

		QueueRunNext();
	}

	public List<UsageParameters> GetQueue()
		=> Queue;

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

	public void QueueRunNext()
	{
		if (Queue.Count == 0) { throw new Exception("Nothing to run."); }

		UsageParameters parametersToUse = Queue.First();
		if (parametersToUse.Cancelled)
			throw new Exception("This was cancelled!");

		//Use it
			if (!parametersToUse.Cancelled)
			{
				EventBus.ActionPreUsed?.Invoke(parametersToUse);
				parametersToUse.ActionRef.Use(parametersToUse);
				parametersToUse.Used = true;
				MsgLog.LogGameMsg(parametersToUse.ActionRef.GetUseText(parametersToUse));
			}
		
		EventBus.ActionEventQueueFinished?.Invoke(Queue);

		MsgLog.LogInfoMsg($"Ran queued actions: {Queue.ToStringList()}");

		QueueClear();
	}
	#endregion

	#region Event Handling
	private void OnBattleStateChanged(ECombatState state)
	{
		if (state == ECombatState.ACTION_RUNNING)
		{
			QueueProcessing = true;
		}
	}

	private void OnActionQueueRequested(UsageParameters parameters)
	{
		QueueAdd(parameters);
	}
    #endregion
}
