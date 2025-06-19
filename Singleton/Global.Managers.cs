using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Shared.Storage;
using Godot;

public partial class Global
{
    public static ResourcePack<Job> ManagerJob;
    public static ResourcePack<Ability> ManagerAbility;
    public static ResourcePack<Mob> ManagerMob;
    public static ResourcePack<Faction> ManagerFaction;
    public static ResourcePack<Inventory> ManagerInventory;
    public static ResourcePack<Item> ManagerItem;

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerItem = new();
    }
}
