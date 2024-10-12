using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections = Godot.Collections;
using Godot;
using ChessLike.Shared.Storage;

[GlobalClass]
public partial class InventorySlotResource : Godot.Resource
{
    [Export]
    public ItemResource? Item;
    [Export]
    public Godot.Collections.Array<EItemFlag> FlagWhitelist = new();
    [Export]
    public Godot.Collections.Array<EItemFlag> FlagBlacklist = new();

}
