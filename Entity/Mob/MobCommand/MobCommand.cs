using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Extension;

namespace ChessLike.Entity.Command;

public abstract class MobCommand
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
    }

    public static void ConnectActionToBroadcaster(Broadcaster.InfoDictFromMob action)
    {
        Broadcaster.CommandUsedOnMob += action;
    }
    public static void ConnectActionToBroadcaster(Broadcaster.InfoDict action)
    {
        Broadcaster.CommandUsed += action;
    }

    public static class Broadcaster
    {
        public delegate void InfoDict(Dictionary<EInfo, string> dictionary);
        public delegate void InfoDictFromMob(Mob victim, Dictionary<EInfo, string> dictionary);

        public static event InfoDictFromMob? CommandUsedOnMob;
        public static event InfoDict? CommandUsed;

        public static void Broadcast(Dictionary<EInfo, string> dictionary)
        {
            CommandUsed?.Invoke(dictionary);
        }

        public static string ParseInfo(Dictionary<EInfo, string> dictionary)
        {
            string output = "";

            foreach (KeyValuePair<EInfo, string> item in dictionary)
            {
                switch(item.Key)
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
}

