using System;
using ChessLike.Shared.Storage;
using Godot;

public class ItemResourcePack : ResourcePack<Item>
{
    public ItemResourcePack() : base() {}
    public ItemResourcePack(string uniqueString) : base(uniqueString)
    {

    }
}
