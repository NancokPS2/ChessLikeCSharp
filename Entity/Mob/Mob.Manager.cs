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

        public List<Mob> GetFromFaction(EFaction faction)
        {
            return GetAllPooled().FilterFromFaction(faction);
        }

        //TODO
        public List<Mob> GetInCombat()
        {
            return GetAllPooled().FilterFromState(EMobState.COMBAT);
        }

        /// <summary>
        /// Gets all mobs in the given location that are in combat.
        /// </summary>
        /// <param name="position">The location to look for mobs in.</param>
        /// <returns>A list of mobs found</returns>
        public List<Mob> GetInPosition(Vector3i position)
        {
            return GetAllPooled().FilterFromPosition(position).FilterFromState(EMobState.COMBAT);
        }

    }
}
public static class MobListExtension
{
    public static List<Mob> FilterFromHostilesFaction(this List<Mob> @this, EFaction main_faction_key)
    {
        List<Mob> output = new();
        Faction main_faction = Global.ManagerFaction.GetFromEnum(main_faction_key) ?? throw new Exception("No faction exists with this enum");

        foreach (var mob in @this)
        {
            Faction other_faction = Global.ManagerFaction.GetFromEnum(mob.Faction);
            if (main_faction.IsEnemy(other_faction.Identifier))
            {
                output.Add(mob);
            } 
        }

        return output;
    }
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