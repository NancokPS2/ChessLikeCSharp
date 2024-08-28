using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class MobManager
{
    public static Dictionary<Faction> factions = new();
    private Dictionary<Mob, MobStatus> mobs = new();

    public void AddMob(Mob mob)
    {
        if (mobs.Keys.Contains(mob))
        {
            throw new Exception("Mob already added.");
        }else
        {
            mobs.Add(mob, new());
        }

    }
    public void GetMobsFromFaction(Faction faction)
    {
        
    }

    public void SaveAll()
    {

    }

    private struct MobStatus
    {
        bool Benched = true;
        Faction faction;


        public MobStatus(Mob mob)
        {

        }
    }
}
