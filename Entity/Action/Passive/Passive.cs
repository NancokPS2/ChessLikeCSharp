using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.World;
using Godot;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

public partial class Passive : ActionEvent
{
    public EPassive Identifier = EPassive.DAMAGE_HALVED;
    public bool Active = true;

    public bool IsTriggeredByPassive = false;
    public bool IsTriggeredByTurnEnd = false;

    public DurationParameters DurationParams = new DurationParameters(null, null, null);
    
    public List<EActionFlag> FlagsForTrigger = new();

    public UsageParameters BaseParameters { get => baseParameters; set => baseParameters = value; }
    private UsageParameters baseParameters;



    public Passive() : base()
    {
        BaseParameters = new(Owner, BattleController.CompGrid, this);
        EventBus.AbilityUsed += OnAbilityUsed;

    }

    private void OnAbilityUsed(UsageParameters parameters)
    {
        if (IsTriggeredByUse(parameters))
        {
            Use(BaseParameters);
        }
    }

    protected virtual bool IsTriggeredByUse(UsageParameters parameters){return false;}

    public override void Use(UsageParameters usage_params)
    {
        DurationParams.AdvanceUses();
        EventBus.AbilityUsed?.Invoke(usage_params);
    }

    public override string GetDescription() => "\n" + GetDurationDescription();

    protected virtual string GetDurationDescription()
    {
        string output = "";
        if (DurationParams.Turns.GetTimeLeft() > 0)
        {
            output += $"Remaining turns: {DurationParams.Turns}\n";
        }
        if (DurationParams.Delay.GetTimeLeft() > 0)
        {
            output += $"Remaining delay: {DurationParams.Delay}\n";
        }
        if (DurationParams.Uses.GetTimeLeft() > 0)
        {
            output += $"Remaining uses: {DurationParams.Uses}\n";
        }
        return output;
    }
}