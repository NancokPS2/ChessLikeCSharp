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

    private UsageParameters BaseParams;

    public Passive() : base()
    {
        BaseParams = new(Owner, BattleController.CompGrid, this);
    }

    public virtual UsageParameters GetUsageParams()
    {
        return BaseParams;
    }

    public UsageParameters GenerateUsageParameters()
    {
        throw new NotImplementedException();
    }

    public bool IsTriggeredByEvent(ActionEvent action)
    {
        //If the action is a passive and those are not being considered, pass.
        if (action is Passive && !IsTriggeredByPassive)
        {
            return false;
        }
        //The action must contain all flags required for the trigger
        else if (!action.Flags.ContainsAll(FlagsForTrigger) )
        {
            return false;
        }

        return true;
    }


    public override void Use(UsageParameters usage_params)
    {
        DurationParams.AdvanceUses();
    }
}
public static class Extension
{
    public static List<Passive> FilterTriggeredByEvent(this List<Passive> passives, ActionEvent action)
    {
        List<Passive> output = new();
        foreach (var item in passives)
        {
            if (item.IsTriggeredByEvent(action))
            {
                output.Add(item);
            }
        }
        return output;
    }

}
