using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    public class Manager : Manager<Mob>
    {
        public Dictionary<EMob, Mob> PrototypeDict = new();

        public List<Mob> GetInFaction(EFaction faction)
        {
            return Contents.FilterFromFaction(faction);
        }

        //TODO
        public List<Mob> GetInCombat()
        {
            return Contents.Where(x => true).ToList();
        }

        public List<Mob> GetInPosition(Vector3i position)
        {
            return Contents.FilterFromPosition(position);
        }
    }
}
public static class MobListExtension
{
    public static List<Mob> FilterFromFaction(this List<Mob> @this, EFaction faction)
    {
        return @this.Where(x => x.Faction == faction).ToList();
    }
    public static List<Mob> FilterFromPosition(this List<Mob> @this, Vector3i position)
    {
        return @this.Where(x => x.Position == position).ToList();
    }
}