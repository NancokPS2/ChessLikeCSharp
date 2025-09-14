using System;
using ChessLike.Entity;
using Godot;

public class FactionResourcePack : ResourcePack<Faction>
{
	public Faction ResourceGet(EPackIDFaction enu)
	{
		string output = Enum.GetName<EPackIDFaction>(enu) ?? throw new Exception("Could not get name from enum.");
		return ResourceGet(output, true);
	}

	public Faction ResourceGet(EFaction faction, bool persistent)
	{
		return ResourceGet(FindIdentifier(faction) ?? throw new Exception(), persistent);
	}

	public string? FindIdentifier(EFaction faction)
	{
		foreach (var item in ContentBase)
		{
			if (item.Value.Identifier == faction) return item.Key;
		}

		return null;
	}

}


