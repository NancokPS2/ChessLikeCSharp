using ChessLike.Entity;
using ChessLike.Extension;
using Godot;
using static ChessLike.Entity.Mob;

namespace ChessLike.Storage;

[GlobalClass]
public partial class Item : Resource, IValuable, IDescription
{

    [Export]
    public string Name = "";

    [Export]
    public float Price = 0;

	[Export]
	public PackedScene? Model;

	[Export]
	public Texture2D Texture;

    [Export]
    public Godot.Collections.Array<EItemFlag> Flags = new();

    [Export]
    public float Value { get => Price; set => Price = value; }

	public Item()
	{
		Texture = new PlaceholderTexture2D(){Size = new(32,32)};
	}

    public void ClearFlags()
	{
		Flags.Clear();
	}

    public void AddFlag(EItemFlag flag)
    {
        Flags.Add(flag);
    }

    public void RemoveFlag(EItemFlag flag)
    {
        Flags.Remove(flag);
    }

    public List<EItemFlag> GetFlags()
    {
        return new(Flags);
    }

    public virtual string GetDescription(bool extended = true)
        => $"{Name} \nValue: {Value} \nFlags: {Flags.ToStringList()} \n";

    public string GetDescriptiveName()
    {
        return Name;
    }

	public override string ToString() => GetDescription();

}



