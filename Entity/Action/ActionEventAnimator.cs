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
        if (Queue.Count == 0) throw new Exception("Empty queue.");

        CurrentIndex = 0;
        CurrentAnimationTime = 0;
        Queue = parameterList;

        StartAnimation();
    }

    protected void NextAnimation()
    {
        CurrentIndex++;
 
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

    public float GetAnimationTimeLeft(UsageParameters parameters)
        => parameters.ActionRef.AnimationParams.MaxDuration;

    protected bool IsValidAnimationIndex() => CurrentIndex < Queue.Count;

    protected void EndAnimationQueue()
    {
        CurrentIndex = 0;
        CurrentAnimationTime = 0;
        CurrentParameters = null;
        Queue.Clear();
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

        if (animationParams.FloatingText != "")
        {
            Godot.PopupText3D text = Readonly.Scenes.SCENE_PARTICLE_POPUP_TEXT;
            Global.GetRoot().AddChild(text);
            text.GlobalPosition = owner.GetNode().MarkerOverhead.GlobalPosition;
        }
    }

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
