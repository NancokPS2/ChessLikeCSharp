using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

/// <summary>
/// Animates particles and other effects from the ActionEvent.
/// </summary>
[GlobalClass]
public partial class ActionEventAnimator : Node3D, IDebugDisplay
{
    private UsageParameters? CurrentParameters;
    private int CurrentIndex;
    private float CurrentAnimationTime;
    private List<UsageParameters> Queue = new();

    public ActionEventAnimator()
    {
        EventBus.ActionEventQueueFinished += OnActionEventQueueFinished;
    }

    #region Animation
    protected void StartAnimationQueue(List<UsageParameters> parameterList)
    {
        if (parameterList.Count == 0) throw new Exception("No parameters where received.");

        CurrentIndex = 0;
        CurrentAnimationTime = 0;
        Queue = parameterList;

        StartAnimation();
    }

    protected void StartAnimation()
    {
        if (!IsValidAnimationIndex()) throw new Exception();
        StartAnimation(Queue[CurrentIndex]);
    }

    protected void StartAnimation(UsageParameters parameters)
    {
        CurrentAnimationTime = 0;
        CurrentParameters = parameters;

        AnimationParameters animationParams = parameters.ActionRef.AnimationParams;
        ActionEvent action = parameters.ActionRef;
        Mob owner = parameters.OwnerRef;

        EventBus.ActionAnimationStarted?.Invoke(CurrentParameters);

        //WIP need to animate this.
        action.AnimationRun(parameters);
    }
    
    protected void EndAnimationQueue()
    {
        CurrentIndex = 0;
        CurrentAnimationTime = 0;
        CurrentParameters = null;
        EventBus.ActionAnimationQueueEnded?.Invoke(Queue);
        Queue.Clear();
    }

    protected void NextAnimation()
    {
        CurrentIndex++;

        EventBus.ActionAnimationEnded?.Invoke(
            CurrentParameters
            ?? throw new Exception("How did it continue if it was not playing one already?")
            );

        if (!IsValidAnimationIndex())
        {
            EndAnimationQueue();
            return;
        }
        else
        {
            StartAnimation();
        }

    }

    protected bool IsValidAnimationIndex() => CurrentIndex < Queue.Count;

    public override void _Process(double delta)
    {
        if (CurrentParameters is null)
            return;

        if (CurrentParameters.ActionRef.AnimationParams.MaxDuration > CurrentAnimationTime)
            NextAnimation();

        CurrentAnimationTime += (float)delta;
    }
    #endregion

    #region Misc
    public string GetText()
    {
        string output = string.Format(
            "Running ability: {0} \nRunning time: {1} \nRunning index: {2}",
            new object?[]{
                CurrentParameters is not null ? CurrentParameters.ActionRef.Name : "null",
                CurrentAnimationTime,//RunningTime.ToString(),
                CurrentIndex.ToString(),
            });

        return output;
    }
    #endregion

    #region Event Connection

    private void OnActionEventQueueFinished(List<UsageParameters> parameterList)
    {
        StartAnimationQueue(parameterList);
    }

    #endregion
}
