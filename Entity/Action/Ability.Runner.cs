using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Turn;

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

    private UniqueList<Ability> PassivesTracked = new();

    /// <summary>
    /// ONLY accessible when adding a passive.
    /// </summary>
    private Dictionary<Ability, int> PassiveTurnsPassed = new();

    public void PassiveTrack(Ability ability)
    {
        if (!PassiveIsValid(ability)){return;}

        PassivesTracked.Add(ability);
        PassiveTurnsPassed[ability] = 0;
    }

    public void PassiveTrackStop(Ability ability)
    {
        if(!PassivesTracked.Contains(ability)){throw new Exception("This passive wasn't being tracked in the first place.");}
        PassivesTracked.Remove(ability);
        PassiveTurnsPassed.Remove(ability);
    }

    public bool PassiveIsValid(Ability ability)
    {
        return ability.PassiveParams.DurationTurn > 0;
    }

    public void PassiveTrack(Mob mob)
    {
        foreach (var item in mob.GetAbilities())
        {
            PassiveTrack(item);
        }
    }

    /// <summary>
    /// Ran from BattleController
    /// </summary>
    public void PassiveTurnTick(Mob turn_ender)
    {
        List<Ability> to_not_track = new();
        foreach (var item in PassivesTracked)
        {
            //Skip if this isn't from the one that ended their turn.
            if(item.Owner != turn_ender){continue;}

            PassiveTurnsPassed[item] += 1;
            if (PassiveTurnsPassed[item] >= item.PassiveParams.DurationTurn)
            {
                to_not_track.Add(item);
            }
        }

        foreach (var item in to_not_track)
        {
            PassiveTrackStop(item);
        }
    }

    public void PassiveTurnTick(ITurn turn_ender) => PassiveTurnTick((Mob)turn_ender);

    public uint QueueAdd(Ability action, Ability.UsageParams parameters)
    {
        if(parameters.PositionsTargeted.Count == 0){throw new Exception("Invalid parameters.");}
        uint id = QueueGetAvailableId();
        int index = Queue.Count;
        return QueueInsert(action, parameters, index);
    }

    private uint QueueInsert(Ability action, Ability.UsageParams parameters, int index)
    {
        uint id = QueueGetAvailableId();
        Queue.Insert(index, new(action, parameters, id));
        ActionQueued?.Invoke(action, parameters);
        return id;
    }

    public bool QueueIsEmpty() => Queue.Count == 0;

    private QueuedAction? GetById(uint id)
    {
        return Queue.First(x => x.id == id);
    }

    private uint QueueGetAvailableId()
    {
        uint id = 0;
        while (Queue.Any( x => x.id == id))
        {
            id ++;
        }
        return id;
    }

    public void QueueClear()
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

    public void Process(float delta)
    {
        //If not allowed to run, stop.
        if (!RunningEnabled){return;}

        bool current_expired = RunningQueuedAction is not null && RunningTime > RunningQueuedAction.action.AnimationParams.Duration;

        //If it reached the end, stop.
        if (RunningIndex >= Queue.Count)
        {
            //If one is still running, wait for it.
            //if (!current_expired){goto tick;}

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
            RunningReadyToReplace = false;
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
        QueueClear();
        RunningEnabled = false;
        RunningIndex = 0;
        RunningTime = 0;
        QueueEnded?.Invoke();
    }

    public string GetText()
    {
        string output = string.Format(
            "Running ability: {0} \nRunning time: {1} \nRunning index: {2} \nQueue count: {3}",
            new object?[]{
                RunningQueuedAction is not null ? RunningQueuedAction.action.Name : "null", 
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
