using System;
using ChessLike.Entity;
using Godot;

public class MobResourcePack : ResourcePack<Mob>
{
    public Mob GetResource(EPackIDMob enu)
    {
        string? output = Enum.GetName<EPackIDMob>(enu);
        return ResourceGet(output ?? throw new Exception("Could not get name from enum."));
    }

    [Obsolete("This is not safe enough.")]
    public override string GetResourceIdentifier(Mob resource)
    {
        string output = base.GetResourceIdentifier(resource);
        if (output == "") output = resource.DisplayedName;
        return output;
    }


    public List<Mob> GetPooledInCombat()
        => PooledGetAll().FilterInCombat();

    public List<Mob> GetPooledInPosition(Vector3i position)
        => PooledGetAll().FilterInPosition(position: position);

    public List<Mob> GetPooledInFaction(EFaction faction)
        => PooledGetAll().FilterInFaction(faction);

    public List<Mob> FilterFromHostilesFaction(List<Mob> mobList, EFaction main_faction_key)
    {
        List<Mob> output = new();
        Faction main_faction = Global.ManagerFaction.GetPooledByEnum(main_faction_key) ?? throw new Exception("No faction exists with this enum");

        foreach (var mob in mobList)
        {
            Faction other_faction = Global.ManagerFaction.GetPooledByEnum(mob.Faction);
            if (main_faction.IsEnemy(other_faction.Identifier))
            {
                output.Add(mob);
            }
        }

        return output;
    }

}

public static class ListMobExtension
{
    public static List<Mob> FilterInCombat(this List<Mob> mobs)
        => mobs.Where(x => x.MobState == EMobState.COMBAT).ToList();

    public static List<Mob> FilterInPosition(this List<Mob> mobs, Vector3i position)
        => mobs.Where(x => x.GetPosition() == position).ToList();

    public static List<Mob> FilterInFaction(this List<Mob> mobs, EFaction faction)
        => mobs.Where(x => x.Faction == faction).ToList();
}