using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public class AbilityRunner : IDebugDisplay
{
    public delegate void ActionQueue(Ability action, Ability.UsageParams parameters);
    public delegate void Delegate();
    public event ActionQueue? ActionQueued;
    public event ActionQueue? ActionStarted;
    public event ActionQueue? ActionEnded;

    public event Delegate? QueueStarted;
    public event Delegate? QueueEnded;

    private List<QueuedAction> Queue = new();

    public uint Add(Ability action, Ability.UsageParams parameters)
    {
        if(parameters.PositionsTargeted.Count == 0){throw new Exception("Invalid parameters.");}
        uint id = GetAvailableId();
        int index = Queue.Count;
        return Insert(action, parameters, index);
    }

    private uint Insert(Ability action, Ability.UsageParams parameters, int index)
    {
        uint id = GetAvailableId();
        Queue.Insert(index, new(action, parameters, id));
        ActionQueued?.Invoke(action, parameters);
        return id;
    }

    public bool IsQueueEmpty() => Queue.Count == 0;

    private QueuedAction? GetById(uint id)
    {
        return Queue.First(x => x.id == id);
    }

    private uint GetAvailableId()
    {
        uint id = 0;
        while (Queue.Any( x => x.id == id))
        {
            id ++;
        }
        return id;
    }

    public void Clear()
    {
        Queue.Clear();
    }


    private bool RunningEnabled;
    private int RunningIndex = 0;
    private float RunningTime = 0;
    private QueuedAction RunningQueuedAction;
    private bool RunningReadyToReplace;
    public void RunStart()
    {
        if (Queue.Count == 0) {throw new Exception("Nothing to run.");}
        RunningReadyToReplace = true;
        RunningEnabled = true;
        RunningIndex = 0;
        RunningTime = 0;
        QueueStarted?.Invoke();
    }

    public async void Process(float delta)
    {
        //If not allowed to run, stop.
        if (!RunningEnabled){return;}

        bool current_expired = RunningQueuedAction is not null && RunningTime > RunningQueuedAction.action.AnimationParams.Duration;

        //If it reached the end, stop.
        if (RunningIndex >= Queue.Count)
        {
            //If one is still running, wait for it.
            if (!current_expired){goto tick;}

            RunEnd();
            return;
        }

        //If no action is running, select one and use it to start.
        if (RunningReadyToReplace == true)
        {
            RunningQueuedAction = Queue[RunningIndex];
            Ability action = RunningQueuedAction.action;
            Ability.UsageParams parameters = RunningQueuedAction.usage_params;

            action.Use(parameters);
            ActionStarted?.Invoke(RunningQueuedAction.action, RunningQueuedAction.usage_params);
        }


        //If the animation already played out, pass onto the next.
        if (RunningTime > RunningQueuedAction.action.AnimationParams.Duration)
        {
            RunningReadyToReplace = true;
            RunningTime = 0;
            RunningIndex ++;
            ActionEnded?.Invoke(RunningQueuedAction.action, RunningQueuedAction.usage_params);
        }

        tick:
        RunningTime += delta;
    }

    private void RunEnd()
    {
        Clear();
        RunningEnabled = false;
        RunningIndex = 0;
        RunningTime = 0;
        QueueEnded?.Invoke();
    }

    public string GetText()
    {
        string output = string.Format(
            "Running action: {0} \nRunning time: {1} \nRunning index: {2} \nQueue count: {3}",
            new object?[]{
                RunningQueuedAction is not null ? RunningQueuedAction.ToString() : "null", 
                RunningTime.ToString(),
                RunningIndex.ToString(),
                Queue.Count.ToString(),
            });

        return output;
    }

    private class QueuedAction
    {
        public Ability action;
        public Ability.UsageParams usage_params;
        public uint id;

        public QueuedAction(Ability action, Ability.UsageParams usage_params, uint id)
        {
            this.action = action;
            this.usage_params = usage_params;
            this.id = id;
        }
    }
}
