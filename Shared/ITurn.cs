using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface ITurn
{
    public float DelayBase {get; set;}
    public float Delay {get; set;}

}
