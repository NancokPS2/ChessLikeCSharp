using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Time;

public interface ITurn
{
    public float DelayCurrent {get;set;}

    public float GetDelayReset();
}
