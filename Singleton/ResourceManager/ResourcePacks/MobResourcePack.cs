using System;
using ChessLike.Entity;
using Godot;

public class MobResourcePack : ResourcePack<Mob>
{
    public List<Mob> GetPooledInCombat()
        => GetAllPooled()
        .Where(x => x.MobState == EMobState.COMBAT)
        .ToList();

    public List<Mob> GetPooledInPosition(Vector3i position)
        => GetAllPooled()
        .Where(x => x.GetPosition() == position)
        .ToList();

    public List<Mob> GetPooledInFaction(EFaction faction)
        => GetAllPooled()
        .Where(x => x.Faction == faction)
        .ToList();


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
