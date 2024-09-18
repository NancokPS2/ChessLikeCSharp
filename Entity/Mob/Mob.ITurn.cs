using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Turn;

namespace ChessLike.Entity;

public partial class Mob : ITurn
{
    public float DelayCurrent { get; set; }
    public float DelayToAddOnTurnEnd { get; set; }

    public float GetDelayBase()
    {
        float stat_delay = Stats.GetValue(StatName.DELAY);
        return stat_delay;
    }
}
