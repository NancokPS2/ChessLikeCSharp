using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class InventoryResource : Godot.Resource
{
    [Export]
    public Godot.Collections.Array<InventorySlotResource> Slots = new();
}
