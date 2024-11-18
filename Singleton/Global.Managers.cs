using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;

public partial class Global
{
    public static Job.Manager ManagerJob;
    public static Ability.Manager ManagerAbility;
    public static Passive.Manager ManagerPassive;
    public static Mob.Manager ManagerMob;
    public static Faction.Manager ManagerFaction;
    public static Item.Manager ManagerItem;

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerPassive = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerItem = new();
    }
}
