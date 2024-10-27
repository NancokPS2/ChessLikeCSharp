using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot.NativeInterop;

namespace ChessLike.Entity.Action;

public partial class Ability
{

    public partial class PassiveParameters
    {

        public bool Active;
        public int DurationTurn = 0;
        public List<EPassiveTrigger> Triggers = new();

        public bool CanTrigger(UsageParams usageParams, EPassiveTrigger trigger) 
        => trigger switch
        {
            EPassiveTrigger.ON_TAKE_DAMAGE => true,
            _ => throw new NotImplementedException(),
        };
    }
    
}
