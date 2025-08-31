using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public abstract partial class Command : Resource
{
    public List<ECommandFlag> Flags = new();

    public Command()
    {
    }

    public virtual void UseCommand(Mob mob)
    {
        EventBus.MobCommandPreUse?.Invoke(this, mob);
    }

    public static string ParseInfo(Dictionary<EInfo, string> dictionary)
    {
        string output = "";

        foreach (KeyValuePair<EInfo, string> item in dictionary)
        {
            switch (item.Key)
            {
                case EInfo.DAMAGE_DEALT:
                    output += "Dealt " + item.Value.ToString() + " damage.";
                    break;

                default:
                    output += " And something mysterious.";
                    break;
            }
        }

        return output;
    }

}

