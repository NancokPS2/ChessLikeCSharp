using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

[GlobalClass]
public partial class InventorySlotResource : Godot.Resource
{
    [Export]
    public ItemResource? Item;
    public Godot.Collections.Array<EItemFlag> FlagWhitelist = new();
    public Godot.Collections.Array<EItemFlag> FlagBlacklist = new();

}
