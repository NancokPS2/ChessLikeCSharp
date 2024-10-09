using ChessLike.Shared.Storage;
using Godot;

[GlobalClass]
public partial class ItemResource : Resource
{
    [Export]
    public string Name = "";
    [Export]
    public Godot.Collections.Array<EItemFlag> Flags = new();
    [Export]
    public float Price;
}
