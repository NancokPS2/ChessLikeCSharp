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
    public int DurationTurn = 0;
    public List<EPassiveTrigger> PassiveTriggers = new();

    public bool CanTrigger(UsageParams usageParams, EPassiveTrigger trigger) 
    => trigger switch
    {
        EPassiveTrigger.ON_TAKE_DAMAGE => true,
        _ => throw new NotImplementedException(),
    };
}
