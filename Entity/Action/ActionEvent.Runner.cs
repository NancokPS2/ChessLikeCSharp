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
    public event ActionQueue? ActionQueued;
    public event ActionQueue? ActionStarted;
    public event ActionQueue? ActionEnded;

    public event Delegate? QueueStarted;
    public event Delegate? QueueEnded;

    private List<QueuedAction> Queue = new();

    private List<Passive> PassiveGetAll()
    {
        List<Passive> output = new();
        foreach (var item in Global.ManagerMob.GetInCombat())
        {
            //Skip if out of combat
            if (item.MobState != EMobState.COMBAT){continue;}

            output.AddRange(PassiveGetFromMob(item));

        } 
        return output;
    }

    private List<Passive> PassiveGetFromMob(Mob mob)
    {
        List<Passive> output = new();
        foreach (var passive in mob.GetPassives())
        {
            if (PassiveIsValid(passive)){output.Add(passive);}
        }
        return output;
    }

    private bool PassiveIsValid(Passive passive)
    {
        bool not_finished = !passive.DurationParams.IsFinished();
        bool active = passive.Active;
        return not_finished && active;
    }

    public uint QueueAdd(ActionEvent action, Ability.UsageParameters parameters)
    {
        if(parameters.PositionsTargeted.Count == 0){throw new Exception("Invalid parameters.");}
        uint id = QueueGetAvailableId();
        int index = Queue.Count;
        return QueueInsert(action, parameters, index);
    }

    private uint QueueInsert(ActionEvent action, Ability.UsageParameters parameters, int index)
    {
        uint id = QueueGetAvailableId();
        Queue.Insert(index, new QueuedAction(action, parameters, id));
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

    private void QueueClear()
    {
        Queue.Clear();
    }

    
    // RUN LOGIC

    private bool RunningEnabled;
    private int RunningIndex = 0;
    [Obsolete("Pending removal.")]
    private float RunningTime = 0;
    private QueuedAction RunningQueuedAction;
    private bool RunningReadyToSet;
    private List<Passive> RunningConsideredPassives = new();
    public void RunStart()
    {
        if (Queue.Count == 0) {throw new Exception("Nothing to run.");}
        RunningReadyToSet = true;
        RunningEnabled = true;
        RunningIndex = 0;
        RunningTime = 0;
        RunningConsideredPassives = PassiveGetAll();
        QueueStarted?.Invoke();
    }

    public void Process()
    {
        //If not allowed to run, stop.
        if (!RunningEnabled){return;}

        //If it reached the end, stop.
        if (RunningIndex >= Queue.Count)
        {
            RunEnd();
            return;
        }

        //If no action is running, select one and use it to start.
        if (RunningReadyToSet == true)
        {
            RunningQueuedAction = Queue[RunningIndex];
            ActionEvent action = RunningQueuedAction.action;
            Ability.UsageParameters parameters = RunningQueuedAction.usage_params;

            action.Use(parameters);
            MessageQueue.AddMessage(action.GetUseText(parameters));
            ActionStarted?.Invoke(RunningQueuedAction.action, RunningQueuedAction.usage_params);
            
            //Check if the action triggers any passive and add them to the queue if they do..
            var triggered = RunningConsideredPassives.FilterTriggeredByEvent(action);
            foreach (Passive item in triggered)
            {
                QueueInsert(item, item.GetUsageParams(), RunningIndex + 1);
            }
            
            RunningReadyToSet = false;
        }


        //If the animation already played out, pass onto the next action.
        if (RunningTime > RunningQueuedAction.action.AnimationParams.Duration)
        {
            RunningReadyToSet = true;
            RunningTime = 0;
            RunningIndex ++;
            ActionEnded?.Invoke(RunningQueuedAction.action, RunningQueuedAction.usage_params);
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
        RunningConsideredPassives.Clear();
        QueueEnded?.Invoke();
    }

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

    public void OnTurnEnded(ITurn who)
    {
        if (who is Mob mob)
        {
            List<Passive> passives = PassiveGetFromMob(mob);
            foreach (var item in passives)
            {
                item.DurationParams.AdvanceTurns();
                if (item.IsTriggeredByTurnEnd)
                {
                    QueueAdd(item, item.GetUsageParams());
                }
            }

        } else
        {
            throw new Exception("Only works on mobs.");
        }
        

    }

    private class QueuedAction
    {
        public ActionEvent action;
        public Ability.UsageParameters usage_params;
        public uint id;

        public QueuedAction(ActionEvent action, Ability.UsageParameters usage_params, uint id)
        {
            this.action = action;
            this.usage_params = usage_params;
            this.id = id;
        }
    }
}
