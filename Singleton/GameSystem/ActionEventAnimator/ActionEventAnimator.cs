using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Animation;
using ChessLike.WorldMap;
using Godot;
using static ChessLike.Entity.Action.AnimatedSceneParameters;

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
        EventBus.EncounterLoaded += OnEncounterLoaded;
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
		Godot.Vector3 ownerPosition = CurrentGridNode.MapToGlobal(owner.GetPosition());

		EventBus.ActionAnimationStarted?.Invoke(CurrentParameters);

		//WIP need to animate this.
		//Spawn scenes.
		AnimateScenes(parameters);
		action.AnimationRun(parameters);
	}

	private void AnimateScenes(UsageParameters parameters)
	{
		AnimationParameters animationParams = parameters.ActionRef.AnimationParams;
		ActionEvent action = parameters.ActionRef;
		Mob owner = parameters.OwnerRef;
		Godot.Vector3 ownerPosition = CurrentGridNode.MapToGlobal(owner.GetPosition());

        foreach (AnimatedSceneParameters sceneSpawn in animationParams.SceneSpawns)
        {
            float duration = sceneSpawn.Duration;
            PackedScene scene = sceneSpawn.Scene;
            var motionModes = sceneSpawn.MotionMode;
            var spawnMode = sceneSpawn.SpawnMode;
            List<(Node3D, Mob, AnimationComponentController)> instanceTuples = new();

            //Create instances of the animations
            foreach (var count in Enumerable.Range(0, sceneSpawn.SpawnCount))
            {
                foreach (var targeted in parameters.MobsTargeted)
                {
                    Node3D instance = scene.Instantiate<Node3D>();
                    Tween tween = instance.CreateTween().SetParallel(true);
                    instanceTuples.Add(
                        (instance, targeted, new AnimationComponentController())
                        );
                }
            }

            //Change spawn position
            switch (spawnMode)
            {
                case ESpawn.SPAWN_AT_OWNER:
                    instanceTuples
                        .ForEach(
                            x => x.Item1.SetGlobalPositionForced(ownerPosition, this)
                            );
                        
                    break;

                case ESpawn.SPAWN_AT_TARGET:
                    foreach (var targetMob in parameters.MobsTargeted)
                    {
                        Godot.Vector3 targetPos = CurrentGridNode.MapToGlobal(targetMob.GetPosition());
                        instanceTuples
                            .ForEach(
                                x => x.Item1.SetGlobalPositionForced(targetPos, this)
                                );
                    }
                    break;

                default: throw new Exception();
            }

            //Apply motion
            foreach (var motion in motionModes)
            {
                switch (motion)
                {
                    case EMotion.STILL:
                        break;

                    case EMotion.MOVE_TO_TARGET:
                        foreach (var tuple in instanceTuples)
                        {
                            Node3D instance = tuple.Item1;
                            Mob targetMob = tuple.Item2;
                            Godot.Vector3 globalPos = CurrentGridNode.MapToGlobal(targetMob.GetPosition());
                            tuple.Item3.Components.Add(
                                new AnimCompAdvance() { TargetGlobal = globalPos, Duration = duration }
                                );
                        }
                        break;

                    case EMotion.RISE_AND_FALL:
                        foreach (var tuple in instanceTuples)
                        {
                            Node3D instance = tuple.Item1;
                            Mob targetMob = tuple.Item2;
                            Godot.Vector3 globalPos = CurrentGridNode.MapToGlobal(targetMob.GetPosition());
                            tuple.Item3.Components.Add(
                                new AnimCompBobbing() { Duration = duration }
                                );
                        }
                        break;

                    case EMotion.SPIN:
                        foreach (var tuple in instanceTuples)
                        {
                            Node3D instance = tuple.Item1;
                            tuple.Item3.Components.Add(new AnimCompSpin());
                        }
                        break;
                    
                    case EMotion.FACE_TRAJECTORY:
                        foreach (var tuple in instanceTuples)
                        {
                            Node3D instance = tuple.Item1;
                            tuple.Item3.Components.Add(new AnimCompFaceTravelDirection());
                        }
                        break;

                    default: throw new Exception();
                }

                //Process the ready tuples
                foreach (var tuple in instanceTuples)
                {
                    //Set up the AnimationComponentController
                    tuple.Item3.AutoFreeOnFinish = sceneSpawn.FreeAfterDuration;
                    tuple.Item3.DurationMax = duration;
                    tuple.Item3.Finished += tuple.Item1.QueueFree;

                    //Add all nodes.
                    AddChild(tuple.Item1);
                    tuple.Item1.AddChild(tuple.Item3);
                    foreach (var animComp in tuple.Item3.Components)
                    {
                        tuple.Item1.AddChild(animComp);
                    }
                }

            }
        }
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

    #region Event Handling

    private void OnActionEventQueueFinished(List<UsageParameters> parameterList)
    {
        StartAnimationQueue(parameterList);
    }

	private void OnEncounterLoaded(EncounterData data)
	{
        CurrentGridNode = CombatScene.GetGridNode();
	}
    #endregion
}
