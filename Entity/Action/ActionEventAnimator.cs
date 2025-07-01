using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public class ActionEventAnimator : IDebugDisplay
{
    private UsageParameters runningQueuedAction;
    private int runningIndex;
    private float timeAnimating;

    public void StartAnimation(UsageParameters parameters)
    {
        throw new NotImplementedException();
    }

    public string GetText()
    {
        string output = string.Format(
            "Running ability: {0} \nRunning time: {1} \nRunning index: {2}",
            new object?[]{
                runningQueuedAction is not null ? runningQueuedAction.ActionRef.Name : "null",
                timeAnimating,//RunningTime.ToString(),
                runningIndex.ToString(),
            });

        return output;
    }
}
