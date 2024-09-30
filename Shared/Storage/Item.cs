namespace ChessLike.Shared.Storage;
public abstract class Item : IValuable
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

}



