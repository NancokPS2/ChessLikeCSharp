using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public class ActionEventAnimator : IDebugDisplay
{
    private UsageParameters? runningQueuedAction;
    private int animatingIndex;
    private float animationCurrentTime;
    private List<UsageParameters> animationQueue = new();
    private bool animationEnabled;

    protected void StartAnimationQueue(List<UsageParameters> parameterList)
    {
        animatingIndex = 0;
        animationCurrentTime = 0;
        animationEnabled = true;
        animationQueue = parameterList;
    }

    protected void Process(double delta)
    {
        if (!animationEnabled) return;



        //End if it is over.
        if (animatingIndex == animationQueue.Count)
        {
            EndAnimationQueue();
        }
    }

    protected void EndAnimationQueue()
    {
        animatingIndex = 0;
        animationCurrentTime = 0;
        animationEnabled = false;
        animationQueue.Clear();
    }

    public void StartAnimation(UsageParameters parameters)
    {
        AnimationParameters animation = parameters.ActionRef.AnimationParams;
        Mob owner = parameters.OwnerRef;
        if (animation.FloatingText != "")
        {
            Godot.PopupText3D text = Readonly.Scenes.SCENE_PARTICLE_POPUP_TEXT;
            Global.GetRoot().AddChild(text);
            text.GlobalPosition = owner.GetNode().MarkerOverhead.GlobalPosition;
        }
    }

    public string GetText()
    {
        string output = string.Format(
            "Running ability: {0} \nRunning time: {1} \nRunning index: {2}",
            new object?[]{
                runningQueuedAction is not null ? runningQueuedAction.ActionRef.Name : "null",
                animationCurrentTime,//RunningTime.ToString(),
                animatingIndex.ToString(),
            });

        return output;
    }
}
