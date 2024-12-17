using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Command;

namespace ChessLike.Entity;

public partial class Mob
{
    private UniqueList<MobCommandInterceptor> CommandInterceptors = new();
    public void CommandAddInterceptor(MobCommandInterceptor command)
    {
        CommandInterceptors.Add(command);
    }
    public List<MobCommandInterceptor> CommandGetInterceptors() => CommandInterceptors;

    public bool CommandRemoveInterceptor(MobCommandInterceptor to_remove)
    {
        return CommandInterceptors.Remove(to_remove);
    }

    public void CommandProcess(MobCommand command)
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
