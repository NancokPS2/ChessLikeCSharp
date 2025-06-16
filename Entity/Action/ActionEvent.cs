using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using Godot;

namespace ChessLike.Entity.Action;

public abstract partial class ActionEvent : Resource
{
    private Mob? owner;
    public Mob Owner { get => owner ?? throw new Exception("Owner should be set before usage."); set => owner = value; }

    [Export]
    public string Name = "Undefined Action";

    [Export]
    public Godot.Collections.Array<EActionFlag> Flags = new();

    [Export]
    public Godot.Collections.Array<MobCommand.Command> Commands = new();

    [ExportGroup("Parameters")]
    [Export]
    public MobFilterParameters FilterParams = new();

    [Export]
    public TargetingParameters TargetParams = new();

    [Export]
    public AnimationParameters AnimationParams = new();

    [Export]
    public MobFilterParameters MobFilterParams = new();


    public ActionEvent()
    {
        EventBus.ActionAboutToBeQueued += (x) => AutoActivationProcessReaction(x, false);
        EventBus.ActionQueued += (x) => AutoActivationProcessReaction(x, true);
        EventBus.TurnTimePassed += AutoActivationProcessTimePassed;
        EventBus.TurnChanged += AutoActivationProcessTurn;
    }

    #region Targeting
    public uint GetTotalRange(Mob owner)
    {
        uint output = TargetParams.TargetingRange;
        if (TargetParams.TargetingRangeStatBonus is EStatName stat)
        {
            output += (uint)owner.Stats.GetValue(stat);
        }
        return output;
    }

    public List<Vector3i> GetTargetAblePositions(UsageParameters usage_params)
    {
        //if (usage_params.PositionsTargeted.Count != 0 || usage_params.MobsTargeted.Count != 0){throw new Exception("This should be called BEFORE locations have been chosen.");}

        Vector3i origin = usage_params.OwnerRef.GetPosition();
        Grid grid = usage_params.GridRef;
        List<Vector3i> output = new();
        Mob owner = usage_params.OwnerRef;

        //If it uses pathing, just query that directly and move on.
        if (TargetParams.TargetingUsesPathing)
        {
            output = grid.NavGetPathablePositions(owner);
            return output;
        }

        uint maxRange = GetTotalRange(owner);

        //Get general area for performance reasons.
        output = grid.GetShapeCube(origin, maxRange);

        //Select positions within range.
        output = output.Where(x => x.DistanceManhattanTo(origin) <= maxRange).ToList();

        return output;
    }

    [Obsolete("WIP")]
    public List<Vector3i> GetAoEPositions(UsageParameters usage_params, List<Vector3i> targets)
    {
        if (targets.Count == 0) { throw new Exception("No position to use AoE in."); }

        List<Vector3i> output = new();

        foreach (Vector3i item in targets)
        {
            Vector3i origin = item;
            Grid grid = usage_params.GridRef;
            Vector3i.Rotation rotation = Vector3i.Rotation.UNROTATED;

            output.AddRange(TargetParams.GetTargetingShape(rotation));
        }

        if (output.Count == 0) { GD.PushWarning("Action's AoE is empty. Could not target here. Maybe tweak its TargetingParams."); }//throw new Exception("Nothing to select?");}

        return output;
    }

    #endregion

    #region  Auto Activation
    [Export]
    public AutoActivationParameters AutoActivationParams
    {
        get => autoActivationParams;
        set
        {
            autoActivationParams = value;
            AutoActivationTriggersLeft = AutoActivationParams.AutoActivationMax;
            TimeSinceLastActivation = 0;
        }
    }

    private AutoActivationParameters autoActivationParams = new();

    protected int AutoActivationTriggersLeft;

    protected void AutoActivationRequest(UsageParameters parameters)
    {
        //Can't activate if it ran out.
        if (AutoActivationTriggersLeft <= 0)
        {
            return;
        }

        EventBus.ActionEventQueueRequested?.Invoke(parameters);
        AutoActivationTriggersLeft -= 1;
        TimeSinceLastActivation = 0;
    }

    #region Auto Activation - Reaction
    protected void AutoActivationProcessReaction(UsageParameters parameters, bool afterAction)
    {
        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.ACTION_REACTION) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //Must be the right timing
        if (AutoActivationParams.ActivatedAfterAction != afterAction) return;

        //Must have the right flags.
        if (!AutoActivationParams.IsActionWithValidFlags(parameters.ActionRef)) return;

        //Must be targeting the owner if the condition is true
        if (AutoActivationParams.ActivatedOnlyIfTargetsMe && !parameters.MobsTargeted.Contains(Owner)) return;

        AutoActivationRequest(GetAutoActivationUsageParametersFromReaction(parameters));
    }

    protected virtual UsageParameters GetAutoActivationUsageParametersFromReaction(UsageParameters parameters) => throw new NotImplementedException();
    protected virtual UsageParameters GetAutoActivationUsageParameters() => throw new NotImplementedException();

    #endregion

    #region Auto Activation - Timing

    protected float TimeSinceLastActivation;
    protected void AutoActivationProcessTimePassed(float number)
    {
        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.EVERY_X_TIME) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //Advance time.
        TimeSinceLastActivation += number;

        //If enough time passed, trigger.
        if (TimeSinceLastActivation > AutoActivationParams.ActivatedEveryXTime)
            AutoActivationRequest(GetAutoActivationUsageParameters());

    }

    #endregion

    #region Auto Activation - Turn
    protected void AutoActivationProcessTurn(Mob who, bool started)
    {
        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.TURN_CHANGE) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //If it is only when THIS unit's turn changes, do nothing if false.
        if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && who != Owner) return;

        //Check if it activates on turn end or start.
        if ((AutoActivationParams.ActivatedByTurnEnd && !started) || (AutoActivationParams.ActivatedByTurnStart && started))
        {
            AutoActivationRequest(GetAutoActivationUsageParameters());
        }
    }
    #endregion

    #endregion

    #region General
    public virtual string GetDescription()
    {
        return "Undefined action description.";
    }
    public virtual string GetUseText(UsageParameters parameters)
    {
        return $"{Owner.DisplayedName ?? "ERROR"} did something mysterious to {parameters.MobsTargeted.ToStringList(", ")}";
    }

    public virtual void Use(UsageParameters usageParams)
    {
        EventBus.ActionUsed?.Invoke(usageParams);
    }

    public override string ToString() => Name;
    #endregion
}
