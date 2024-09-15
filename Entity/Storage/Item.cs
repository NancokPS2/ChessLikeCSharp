namespace ChessLike.Entity;
public abstract class Item : IValuable
{

    public string name = "";
    public float price = 0;
    public int slot_size = 1;
    private List<EItem> types = new();

    public float Value { get; set; } = 0;

    public void AddType(EItem type)
    {
        types.Add(type);
    }

    public bool RemoveType(EItem type)
    {
        return types.Remove(type);
    }

    public void ClearTypes()
    {
        types.Clear();
    }

    public List<EItem> GetTypes()
    {
        return types;
    }

}



