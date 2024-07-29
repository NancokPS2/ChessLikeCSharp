namespace ChessLike.Storage;
public abstract class Item : Shared.ITrade
{
    public enum Type
    {
        INVALID, UNKNOWN, CURRENCY, WEAPON, ARMOR
    }

    public string name = "";
    public float price = 0;
    public int slot_size = 1;
    private List<Type> types = new();


    public void AddType(Type type)
    {
        types.Add(type);
    }

    public bool RemoveType(Type type)
    {
        return types.Remove(type);
    }

    public void ClearTypes()
    {
        types.Clear();
    }

    public List<Type> GetTypes()
    {
        return types;
    }

    public float GetValue()
    {
        return price;
    }

    public void SetValue(float val)
    {
        price = val;
    }
}



