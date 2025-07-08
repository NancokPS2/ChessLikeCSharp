using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.World.Encounter;
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
    private GridNode CurrentGridNode;
    private List<UsageParameters> Queue = new();

    public ActionEventAnimator()
    {
        EventBus.ActionEventQueueFinished += OnActionEventQueueFinished;
        EventBus.EncounterLoading += OnEncounterLoading;
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
        //Spawn scenes.
        foreach (var item in action.AnimationParams.ScenesToSpawn)
        {
            switch (item.Key)
            {
                case AnimationParameters.ESceneAnimationMode.SPAWN_AT_OWNER:
                    item.Value.InstantiateAt<Node3D>(
                        this,
                        CurrentGridNode.MapToGlobal(owner.GetPosition())
                        );
                    break;

                case AnimationParameters.ESceneAnimationMode.SPAWN_AT_TARGET:
                    foreach (var targetMob in parameters.MobsTargeted)
                    {
                        Godot.Vector3 globalPos = CurrentGridNode.MapToGlobal(targetMob.GetPosition());
						item.Value.InstantiateAt<Node3D>(
                            this,
                            globalPos
                            );
                    }
                    break;

                case AnimationParameters.ESceneAnimationMode.MOVE_TO_TARGET: throw new NotImplementedException();

                default: throw new Exception();
            }
        }
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

	private void OnEncounterLoading(EncounterData data)
	{
        CurrentGridNode = CombatScene.GetGridNode();
	}
    #endregion
}
