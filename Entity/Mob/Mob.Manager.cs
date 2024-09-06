using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    public static class Manager
    {

        private static UniqueList<Mob> Mobs = new();

        public static void AddMob(Mob mob)
        {
            Mobs.Add(mob);
        }

        public static List<Mob> GetMobs()
        {
            return Mobs;
        }

        public static List<Mob> GetInFaction(EFaction faction)
        {
            return Mobs.FilterFromFaction(faction);
        }

        //TODO
        public static List<Mob> GetInCombat()
        {
            return Mobs.Where(x => true).ToList();
        }

        public static List<Mob> GetInPosition(Vector3i position)
        {
            return Mobs.FilterFromPosition(position);
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
