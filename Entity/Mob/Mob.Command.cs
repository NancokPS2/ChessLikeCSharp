using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Mob
{
    private UniqueList<Interceptor> CommandInterceptors = new();
    public void CommandAddInterceptor(Interceptor command)
    {
        CommandInterceptors.Add(command);
    }
    public List<Interceptor> CommandGetInterceptors() => CommandInterceptors;

    public bool CommandRemoveInterceptor(Interceptor to_remove)
    {
        return CommandInterceptors.Remove(to_remove);
    }

    public void CommandProcess(MobCommand.Command command)
    {
        foreach (var item in CommandInterceptors)
        {
            if (item.CanIntercept(command))
            {
                item.UseInterceptor(command);
            }
        }

        command.UseCommand(this);
    }
}
