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
    public static JobResourcePack ManagerJob;
    public static ResourcePack<Ability> ManagerAbility;
    public static MobResourcePack ManagerMob;
    public static FactionResourcePack ManagerFaction;
    public static ResourcePack<Inventory> ManagerInventory;
    public static ItemResourcePack ManagerItem;
    public static ResourcePack<PackedScene> ManagerModel;
    public static ResourcePack<PackedScene> ManagerParticle;

    public static void SetupManager()
    {
        ManagerJob = new();
        ManagerAbility = new();
        ManagerMob = new();
        ManagerFaction = new();
        ManagerInventory = new();
        ManagerItem = new();
        ManagerModel = new("Model");
        ManagerParticle = new("Particle");
    }
}
