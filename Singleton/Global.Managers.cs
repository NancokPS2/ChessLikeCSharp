using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Storage;
using Action = ChessLike.Entity.Action;

public partial class Global
{
    public static Job.Manager ManagerJob = new();
    public static Action.Manager ManagerAction = new();
    public static Mob.Manager ManagerMob = new();
    public static Faction.Manager ManagerFaction = new();
    public static Item.Manager ManagerItem = new();



    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAction = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerItem = new();
    }
}
