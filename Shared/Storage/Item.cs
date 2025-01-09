using ChessLike.Entity;
using static ChessLike.Entity.Mob;

namespace ChessLike.Shared.Storage;
public partial class Item : IValuable
{

    public string Name = "";
    public float Price = 0;
    public List<EItemFlag> Flags = new();

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
        return Flags;
    }

    public virtual string GetDescription() 
        => $"{Name} \nValue: {Value} \nFlags: {Flags} \n";


}



