using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

public partial class Passive : ActionEvent
{
    public EPassive Identifier = EPassive.HEAL;
    public bool Active = true;

    public bool IsTriggeredByPassive = false;
    //public List<EAbility> AbilityTriggerFlags;
    public DurationParameters DurationParams = new();
    
    public List<EPassiveTrigger> PassiveTriggers = new();

    public UsageParameters GenerateUsageParameters()
    {
        throw new NotImplementedException();
    }

    public bool IsTriggeredByEvent(ActionEvent action)
    {
        if (action is Passive && !IsTriggeredByPassive)
        {
            return false;
        }

        return true;
    }


    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);

        //Advance uses if they are not disabled.
        if (!DurationParams.IsUsesDisabled())
        {
            DurationParams.AdvanceUses();
        }
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
