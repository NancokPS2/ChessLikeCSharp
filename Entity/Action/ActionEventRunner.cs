using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Turn;
using Godot;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEventRunner : Node3D
{
    public delegate void ActionQueue(ActionEvent action, UsageParameters parameters);
    public delegate void Delegate();

    #region Queue
    private List<UsageParameters> Queue = new();
    public bool RunningEnabled;

    public ActionEventRunner()
    {
        EventBus.ActionEventAutoActivated += OneActionEventAutoActivated;
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
        Debug.Assert(Queue[index + 1] == parametersToDisplace, "The displaced parameter should end up AFTER the chosen index.");
        QueueInsert(parametersToAdd, index);
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



    public void RunStart()
    {
        if (Queue.Count == 0) { throw new Exception("Nothing to run."); }

        if (!RunningEnabled) { return; }

        for (int queueIndex = 0; queueIndex < Queue.Count; queueIndex++)
        {
            //Select the action to run.
            UsageParameters parametersToUse = Queue[queueIndex];

            //Use it
            EventBus.ActionPreUsed?.Invoke(parametersToUse);
            parametersToUse.ActionRef.Use(parametersToUse);
            MessageQueue.AddMessage(parametersToUse.ActionRef.GetUseText(parametersToUse));
        }
        EventBus.ActionEventQueueFinished?.Invoke(Queue);

        QueueClear();
    }
    #endregion

    #region Event Connection

    private void OneActionEventAutoActivated(UsageParameters activated, UsageParameters activatedBy)
    {
        QueueAddBefore(activated, activatedBy);
    }

    #endregion
}
