using ChessLike.Shared.Storage;
using Godot;

namespace ChessLike.Entity;

public partial class FactionResource : Resource
{
    [Export]
    public EFaction Identifier;
    [Export]
    public Godot.Collections.Dictionary<EFaction, float> RelationList = new();
    [Export]
    public InventoryResource Inventory = new();
}