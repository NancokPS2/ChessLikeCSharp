using ChessLike.Entity;
using Godot;
using static ChessLike.Entity.Mob;

namespace ChessLike.Storage;
public partial class Item : Resource, IValuable
{

    [Export]
    public string Name = "";

    [Export]
    public float Price = 0;

    [Export]
    public Godot.Collections.Array<EItemFlag> Flags = new();

    [Export]
    public float Value { get => Price; set => Price = value; }

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

    public virtual string GetDescription() 
        => $"{Name} \nValue: {Value} \nFlags: {Flags} \n";


}



