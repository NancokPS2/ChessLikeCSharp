using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Mob
{
    public class Manager : SerializableManager<Mob, MobResource>
    {
        public Dictionary<EMobPrototype, Mob> PrototypeDict = new();

        public List<Mob> NewFromFaction(EFaction faction)
        {
            IEnumerable<Mob>? output =  
                from mob_res 
                in GetAllResources().Where(x => x.Faction == faction) 
                select FromResource(mob_res);
            return output.ToList();
        }

        public List<Mob> GetFromFaction(EFaction faction)
        {
            return GetAllPooled().FilterFromFaction(faction);
        }

        //TODO
        public List<Mob> GetInCombat()
        {
            return GetAllPooled().FilterFromState(EMobState.COMBAT);
        }

        public List<Mob> GetInPosition(Vector3i position)
        {
            return GetAllPooled().FilterFromPosition(position);
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
    public static List<Mob> FilterFromState(this List<Mob> @this, EMobState state)
    {
        return @this.Where(x => x.MobState == state).ToList();
    }
}