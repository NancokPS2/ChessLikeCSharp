using System;
using ChessLike.Entity;
using Godot;

public class FactionResourcePack : ResourcePack<Faction>
{
    public Faction GetPooledByEnum(EFaction faction)
        => GetAllPooled()
        .First(x => x.Identifier == faction);
}
