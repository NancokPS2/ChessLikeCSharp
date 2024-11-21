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
    public LimitParameters LimitParams = new();
    
    public List<EPassiveTrigger> PassiveTriggers = new();

    public UsageParameters GenerateUsageParameters()
    {
        throw new NotImplementedException();
    }

    public bool CanTrigger(UsageParameters usageParams, EPassiveTrigger trigger) 
    => trigger switch
    {
        EPassiveTrigger.ON_TAKE_DAMAGE => true,
        _ => throw new NotImplementedException(),
    };


    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);

        //Advance uses if they are not disabled.
        if (!LimitParams.IsUsesDisabled())
        {
            LimitParams.AdvanceUses();
        }
    }
}
