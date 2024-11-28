using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Command;

public abstract class MobCommandInterceptor
{
    public virtual Dictionary<EInfo, string> GetDefaultInfo()
    {
        Dictionary<EInfo, string> output = new(){
            {EInfo.COMMAND_VERB, "Intercepted"}
        };
        return output;
    }
    public abstract bool CanIntercept(MobCommand command);

    public abstract void UseInterceptor(MobCommand command);
}
