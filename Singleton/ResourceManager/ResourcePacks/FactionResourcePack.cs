using System;
using ChessLike.Entity;
using Godot;

public class FactionResourcePack : ResourcePack<Faction>
{
    private void OnLoadedFromFolder(List<Faction> resources)
    {
        foreach (var item in PooledGetAll())
        {
            PooledAdd(item);
        }
    }

    public Faction ResourceGet(EPackIDFaction enu)
    {
        string? output = Enum.GetName<EPackIDFaction>(enu);
        return ResourceGet(output ?? throw new Exception("Could not get name from enum."), true);
    }

    public Faction GetPooledByEnum(EFaction faction)
        => PooledGetAll()
        .First(x => x.Identifier == faction);

}
