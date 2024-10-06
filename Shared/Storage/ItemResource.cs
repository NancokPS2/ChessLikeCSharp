using Godot;

namespace ChessLike.Shared.Storage;

[GlobalClass]
public partial class ItemResource : Resource
{
    [Export]
    public string Name;
    [Export]
    public Godot.Collections.Array<EItemFlag> Flags = new();
    [Export]
    public float Price;
}
