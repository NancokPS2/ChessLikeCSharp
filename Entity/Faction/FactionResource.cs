using ChessLike.Shared.Storage;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class FactionResource : Resource
{
    [Export]
    public EFaction Identifier;
    [Export]
    public Godot.Collections.Dictionary<EFaction, float> RelationList = new();
    [Export]
    public InventoryResource Inventory = new();
}