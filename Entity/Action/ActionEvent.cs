using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using ExtendedXmlSerializer;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class ActionEvent : Resource
{
    private Mob? owner;
    public Mob Owner
	{
		get
		{
			return owner ?? throw new Exception("Owner should be set before usage.");
		}
		set
		{
			owner = value;
		}
	}

	[Export]
    public string Name = "Undefined Action";

    [Export]
    private Godot.Collections.Array<EActionFlag> flags
    {
        set => Flags = new(value);
        get => new(Flags);
    }
    public List<EActionFlag> Flags = new();

    [Export]
    private Godot.Collections.Array<MobCommand.Command> commands
    {
        set => Commands = new(value);
        get => new(Commands);
    }
    public List<MobCommand.Command> Commands = new();

    [ExportGroup("Parameters")]

    [Export]
    public TargetingParameters TargetParams = new();

    [Export]
    public AnimationParameters AnimationParams = new();

    [Export]
    public MobFilterParameters MobFilterParams = new();

	[Export]
	public CostParameters CostParams = new();


    public ActionEvent()
	{
		EventBus.ActionQueued += OnActionQueued;

		EventBus.TurnTimePassed += OnAutoActivationProcessTimePassed;

		EventBus.MobTurnStarted += OnAutoActivationProcessTurnStarted;
		EventBus.MobTurnEnded += OnAutoActivationProcessTurnEnded;

		EventBus.InputActionSelected += OnInputActionSelected;
	}

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    #region Visual
    public Texture2D GetAnimationFloatingTexture() => throw new NotImplementedException();

    public virtual void AnimationRun(UsageParameters parameters)
    {
        if (parameters.ActionRef != this) throw new Exception();
    }

    #endregion

    #region Targeting
    public int GetMaxTargetingSelections()
        => TargetParams.MaxPositions;

    public uint GetTotalRange(Mob owner)
    {
        uint output = TargetParams.Range;
        if (TargetParams.RangeStatBonus is EStatName stat && stat != EStatName.NONE)
        {
            output += (uint)owner.Stats.GetStat(stat);
        }
        return output;
    }

    //Returns the positions targetable by this action, relative to the owner.
    public List<Vector3i> GetTargetVectors(UsageParameters usageParams)
    {
        if (usageParams.ActionRef != this) throw new Exception();

        Vector3i origin = usageParams.OwnerRef.GetPosition();
        Grid grid = usageParams.GridRef;
        Mob owner = usageParams.OwnerRef;
        List<Vector3i> output = new();

        //If it uses pathing, just query that directly and move on.
        if (TargetParams.UsesPathing)
        {
            output = grid.NavGetPathablePositions(owner);
            return output;
        }

        //Get the shape.
        output = TargetParams.GetTargetingShape();

        //Convert the list to be relative to its owner position.
        output = output.Select(x => x + Owner.GetPosition()).ToList();

        //Make sure they are inbounds
        output = output
            .Where(x => grid.IsPositionInbounds(x))
            .ToList();

        //Select positions within range and filter them.
        uint maxRange = GetTotalRange(owner);
        output = output
            .Where(x => x.DistanceManhattanTo(origin) <= maxRange)
            .ToList();

        return output;
    }

    public List<List<Vector3i>> GetAoEVectors(UsageParameters usageParams, List<Vector3i> selectedPositions)
    {
        if (usageParams.ActionRef != this) throw new Exception();
        if (selectedPositions.Count == 0) throw new Exception("No position to use AoE in.");

        Vector3i origin = usageParams.OwnerRef.GetPosition();
        Grid grid = usageParams.GridRef;
        Mob owner = usageParams.OwnerRef;
        List<List<Vector3i>> output = new();

        foreach (Vector3i selected in selectedPositions)
        {
            List<Vector3i> subOutput = new();

            //Get the rotation to look from the owner to the target. This is relative to the user, so ZERO atm.
            Vector3i.Rotation rotation = Vector3i.ZERO.GetRotationToLookAt(selected, true);

            //Get the shape.
            subOutput = TargetParams.GetAoEShape(rotation);

            //Convert the list to be relative to the selection position.
            subOutput = subOutput.Select(x => x + selected).ToList();

            //Make sure they are inbounds
            subOutput = subOutput
                .Where(x => grid.IsPositionInbounds(x))
                .ToList();

            //Select positions within range and filter them.
            uint maxRange = GetTotalRange(owner);
            subOutput = subOutput
                .Where(x => x.DistanceManhattanTo(origin) <= maxRange)
                .ToList();

            //Add new entries that are not duplicated to output.
            foreach (var cluster in output)
            {
                subOutput = subOutput
                    .Where(x => cluster.Contains(x))
                    .ToList();
            }

            output.Add(subOutput);
        }

        return output;
    }

    #endregion

    #region Mob Filter
    public List<Mob> GetValidMobs(List<Mob> mobPositions)
        => mobPositions.Where(x => IsMobValid(x)).ToList();

    public bool IsMobValid(Mob mob)
    {
        Faction owner_fac = Global.ManagerFaction.ResourceGet(EPackIDFaction.Player);

        //Must be the owner?
        if (mob != Owner && MobFilterParams.OnlyAffectOwner)
        {
            return false;
        }
        //If health is above the max percent, fail.
        else if (mob.Stats.GetValuePrecent(EValueName.HEALTH) > MobFilterParams.MaximumHealthPercent)
        {
            return false;
        }
        //Check for faction
        else if (owner_fac.IsAlly(mob.Faction) && MobFilterParams.CannotAffectAlly)
        {
            return false;
        }
        else if (owner_fac.IsEnemy(mob.Faction) && MobFilterParams.CannotAffectEnemy)
        {
            return false;
        }
        return true;
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
            AutoActivationReset();
        }
    }
    private AutoActivationParameters autoActivationParams = new();

    protected float AutoActivationTimeSinceLast;

    protected int AutoActivationLeft;

    public int GetAutoActivationsLeft() => AutoActivationLeft;

    public void AutoActivationReset()
    {
        AutoActivationLeft = AutoActivationParams.AutoActivationMax;
        AutoActivationTimeSinceLast = 0;
    }

    protected virtual UsageParameters GetAutoActivationUsageParametersFromReaction(UsageParameters parameters)
        => new(Owner, parameters.GridRef, this);
    protected virtual UsageParameters GetAutoActivationUsageParameters()
        => new(Owner, CombatScene.GetGrid(), this);

    protected void AutoActivationRequest(UsageParameters parameters)
    {
        //Can't activate if it ran out.
        if (AutoActivationLeft <= 0)
        {
            throw new Exception("Should already be removed?");
        }

        EventBus.ActionEventAutoActivated?.Invoke(
            GetAutoActivationUsageParametersFromReaction(parameters),
            parameters
            );
        AutoActivationLeft -= 1;
        AutoActivationTimeSinceLast = 0;
    }


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
        if (usageParams.ActionRef != this) throw new Exception();

        foreach (var command in Commands)
        {
            foreach (var mob in usageParams.MobsTargeted)
            {
                command.UseCommand(mob);
            }
        }
        EventBus.ActionUsed?.Invoke(usageParams);
    }

	public bool CanUse() => true;

	public bool IsPassive() => AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.NONE;
    public override string ToString() => Name;
    #endregion

    #region Event Handling

    protected void OnAutoActivationProcessTurnStarted(Mob who)
    {
        //Check if it activates on turn end or start.
        if (!AutoActivationParams.ActivatedByTurnStart) return;

        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.TURN_CHANGE) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //If it is only when THIS unit's turn changes, do nothing if false.
        if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && who != Owner) return;

        AutoActivationRequest(GetAutoActivationUsageParameters());
    }

    protected void OnAutoActivationProcessTurnEnded(Mob who)
    {
        //Check if it activates on turn end or start.
        if (!AutoActivationParams.ActivatedByTurnEnd) return;

        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.TURN_CHANGE) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //If it is only when THIS unit's turn changes, do nothing if false.
        if (AutoActivationParams.ActivatedOnlyIfTurnIsMine && who != Owner) return;

        AutoActivationRequest(GetAutoActivationUsageParameters());
    }

    protected void OnAutoActivationProcessTimePassed(float number)
    {
        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.EVERY_X_TIME) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //Advance time.
        AutoActivationTimeSinceLast += number;

        //If enough time passed, trigger.
        if (AutoActivationTimeSinceLast > AutoActivationParams.ActivatedEveryXTime)
            AutoActivationRequest(GetAutoActivationUsageParameters());

    }

    protected void OnActionQueued(UsageParameters parameters)
    {
        //Must be set to react to actions
        if (AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.ACTION_REACTION) return;

        //The owner must be in combat.
        if (!Owner.IsInCombat()) return;

        //Must have the right flags.
        if (!AutoActivationParams.IsActionWithValidFlags(parameters.ActionRef)) return;

        //Must be targeting the owner if the condition is true
        if (AutoActivationParams.ActivatedOnlyIfTargetsMe && !parameters.MobsTargeted.Contains(Owner)) return;

        AutoActivationRequest(GetAutoActivationUsageParametersFromReaction(parameters));
    }

    private void OnInputActionSelected(ActionEvent obj)
    {
        if (obj != this) return;

        EventBus.TargetingUsageParametersGenerated?.Invoke(
            new(Owner, CombatScene.GetGrid(), this)
        );
	}
    #endregion
}
