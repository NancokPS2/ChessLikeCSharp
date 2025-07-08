using System;
using ChessLike.Entity;
using Godot;

public class FactionResourcePack : ResourcePack<Faction>
{
	private void OnLoadedFromFolder(List<Faction> resources)
	{
        foreach (var item in GetAllPooled())
        {
            AddPooled(item);   
        }
	}

	public Faction GetResource(EPackIDFaction enu)
    {
        string? output = Enum.GetName<EPackIDFaction>(enu);
        return GetResource(output ?? throw new Exception("Could not get name from enum."));
    }

    public Faction GetPooledByEnum(EFaction faction)
        => GetAllPooled()
        .First(x => x.Identifier == faction);
}
