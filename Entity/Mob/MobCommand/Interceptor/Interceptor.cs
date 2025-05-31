using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.MobCommand;

public abstract class Interceptor
{

    public virtual Dictionary<EInfo, string> GetDefaultInfo()
    {
        Dictionary<EInfo, string> output = new(){
            {EInfo.COMMAND_VERB, "Intercepted"}
        };
        return output;
    }

    protected virtual void OnMobCommandUsed(Command mobCommand)
    {
        if (CanIntercept(mobCommand))
        {
            UseInterceptor(mobCommand);
        }
    }

    public abstract bool CanIntercept(Command command);

    public abstract void UseInterceptor(Command command);

}
