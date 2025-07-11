using System;
using ChessLike.Storage;
using Godot;

public class ItemResourcePack : ResourcePack<Item>
{
    public Item GetResource(EPackIDItem enu)
    {
        string? output = Enum.GetName<EPackIDItem>(enu);
        return GetResource(output ?? throw new Exception("Could not get name from enum."));
    }
}
