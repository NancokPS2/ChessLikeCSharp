using ChessLike.Shared.Storage;
using Collections = Godot.Collections;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class FactionResource : Resource
{
    [Export]
    public EFaction Identifier;
    [Export]
    public Collections.Dictionary<EFaction, float> RelationList = new();
    [Export]
    public InventoryResource Inventory = new();
}