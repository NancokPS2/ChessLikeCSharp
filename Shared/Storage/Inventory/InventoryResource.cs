using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Shared.Storage;

public partial class InventoryResource : Godot.Resource
{
    [Export]
    public Godot.Collections.Array<InventorySlotResource> Slots = new();
}
