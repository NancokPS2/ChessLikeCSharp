using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    public class Manager : SerializableManager<Mob>
    {
        public override List<Mob> CreatePrototypes()
        {
            return new()
            {
                new Mob().Create(EMob.HUMAN_COMBATANT)
            };
        }

        public override string GetPrototypeFolder()
        {
            return Path.Combine(
                Global.Directory.GetContentDir(EDirectory.USER_CONTENT),
                "mob"
            );

        }

        public Mob? GetFromIdentifier(EMob identifier)
        {
            return Contents.Find(x => x.Identifier == identifier);
        }

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
