using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChessLike.Turn;
using Godot;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

public class ActionEventRunner : IDebugDisplay
{
    public delegate void ActionQueue(ActionEvent action, UsageParameters parameters);
    public delegate void Delegate();

    #region Queue
    private List<QueuedAction> Queue = new();

    public uint QueueAdd(UsageParameters parameters)
    {
        //Make sure it is valid first.
        if (!parameters.IsValid()) { throw new Exception("Invalid parameters."); }


        //Actually try to queue it.
        int index = Queue.Count;
        return QueueInsert(parameters, index);
    }

    private uint QueueInsert(UsageParameters parameters, int index)
    {
        //Warn other actions about this one, so they can queue first.
        EventBus.ActionAboutToBeQueued?.Invoke(parameters);

        uint id = QueueGetAvailableId();
        Queue.Insert(index, new QueuedAction(parameters.ActionRef, parameters, id));
        EventBus.ActionQueued?.Invoke(parameters);
        return id;
    }

    public bool QueueIsEmpty() => Queue.Count == 0;

    private QueuedAction? GetById(uint id)
        => Queue.First(x => x.id == id);

    private int GetIdOfUsageParameters(UsageParameters parameters)
        => Queue.FindIndex(x => x.usage_params == parameters);

    private uint QueueGetAvailableId()
    {
        uint id = 0;
        while (Queue.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    private void QueueClear()
    {
        Queue.Clear();
    }
    #endregion

    #region Run Logic
    // RUN LOGIC

    private bool RunningEnabled;
    private int RunningIndex = 0;
    //[Obsolete("Pending removal.")]
    private float RunningTime = 0;
    private QueuedAction? RunningQueuedAction;
    private bool RunningReadyToSet;

    public void RunStart()
    {
        if (Queue.Count == 0) { throw new Exception("Nothing to run."); }
        RunningReadyToSet = true;
        RunningEnabled = true;
        RunningIndex = 0;
        RunningTime = 0;
    }

    public void Process()
    {
        //If not allowed to run, stop.
        if (!RunningEnabled) { return; }

        //If it reached the end, stop.
        if (RunningIndex >= Queue.Count)
        {
            RunEnd();
            return;
        }

        //If no action is running, select one and use it to start.
        if (RunningReadyToSet == true)
        {
            //Select the action to run.
            RunningQueuedAction = Queue[RunningIndex];
            //Setup the usage
            ActionEvent action = RunningQueuedAction.action;
            UsageParameters parameters = RunningQueuedAction.usage_params;

            EventBus.ActionAboutToBeUsed?.Invoke(parameters);
            action.Use(parameters);
            MessageQueue.AddMessage(action.GetUseText(parameters));

            RunningReadyToSet = false;
        }


        //If the animation already played out, pass onto the next action.
        float duration = RunningQueuedAction?.action.AnimationParams.Duration ?? throw new Exception("Could not get a duration.");
        if (RunningTime > duration)
        {
            RunningReadyToSet = true;
            RunningTime = 0;
            RunningIndex++;
        }

        //TODO: Maybe don't rely on this random node for timing.
        RunningTime += (float)BattleController.CompDisplayGrid.GetProcessDeltaTime();
    }

    private void RunEnd()
    {
        QueueClear();
        RunningEnabled = false;
        RunningIndex = 0;
        RunningTime = 0;
    }
    #endregion

    #region Misc
    public string GetText()
    {
        string output = string.Format(
            "Running ability: {0} \nRunning time: {1} \nRunning index: {2} \nQueue count: {3}",
            new object?[]{
                RunningQueuedAction is not null ? RunningQueuedAction.action.Name : "null",
                "Disabled",//RunningTime.ToString(),
                RunningIndex.ToString(),
                Queue.Count.ToString(),
            });

        return output;
    }
    #endregion

    #region QueuedAction Class
    private class QueuedAction
    {
        public ActionEvent action;
        public UsageParameters usage_params;
        public uint id;

        public QueuedAction(ActionEvent action, UsageParameters usage_params, uint id)
        {
            this.action = action;
            this.usage_params = usage_params;
            this.id = id;
        }
    }
    #endregion
}
