using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Action = ChessLike.Entity.Action;

public partial class Global
{
    public static Job.Manager ManagerJob;
    public static Action.Manager ManagerAction;
    public static Mob.Manager ManagerMob;
    public static Faction.Manager ManagerFaction;
    public static Item.Manager ManagerItem;

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAction = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerItem = new();
    }
}
