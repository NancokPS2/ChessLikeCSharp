using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.MobCommand;

public abstract partial class Command : Resource
{
    private bool Used = false;

    public List<ECommandFlag> Flags = new();


    public virtual void UseCommand(Mob mob)
    {
        if (Used)
        {
            throw new Exception("Commands are single-use.");
        }
        Used = true;

        EventBus.MobCommandUsed?.Invoke(this, mob);
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

