using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobResource : Resource
{
    [Export]
    public string DisplayedName = "UNNAMED";
    [Export]
    public Godot.Collections.Array<EJob> Jobs = new();
    //[Export]
    //public Godot.Collections.Array<ActionResource> Actions = new();
    [Export]
    public ERace Race;
    [Export]
    public EFaction Faction;
    [Export]    
    public InventoryResource MobInventory = new();
    //[Export]
    //public EMovementMode MovementMode;
    //[Export]
    //public EMobState MobState = EMobState.BENCHED;
    //[Export]
    //public StatSetResource<StatName> StatSet = new();

}
