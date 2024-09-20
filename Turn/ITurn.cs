using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Turn;

public interface ITurn
{
    public event TurnManager.TurnChangeHandler TurnStarted;
    public float DelayCurrent {get;set;}
    public float DelayToAddOnTurnEnd {get;set;}

    public float GetDelayBase();
}
