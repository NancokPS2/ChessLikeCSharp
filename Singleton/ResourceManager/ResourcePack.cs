namespace Godot;

public class ResourcePack<TRes> where TRes : Resource
{
    public readonly string UNIQUE_STRING = GetUniqueString();
    private Dictionary<string, TRes> Contents = new();
    private UniqueList<TRes> Pooled = new();

    public ResourcePack()
    {
    }

    public bool AddPooled(TRes res)
        => Pooled.Add(res);

    public void RemovePooled(TRes res)
        => Pooled.Remove(res);

    public List<TRes> GetAllPooled()
        => Pooled;

    public ResourcePack(string sourceFolder)
        => LoadAllInFolder(sourceFolder);

    static string GetUniqueString()
    {
        //Try to infer it from the type extension
        string output = typeof(TRes).ToString().GetExtension();

        //If empty, get the original as a backup.
        if (output == "") throw new Exception("Could not get an identifier.");//output = typeof(TRes).ToString();

        return output;
    }

    public void RemoveResource(string identifier)
    {
        Contents.Remove(identifier);
    }

    public void AddResource(string identifier, TRes resource, bool replace = false)
    {
        if (Contents.ContainsKey(identifier) && !replace)
        {
            throw new Exception("Resource already exists.");
        }

        Contents[identifier] = resource;
    }

    public TRes GetResource(string identifier, bool getCached = false)
    {
        TRes? output;

        Contents.TryGetValue(identifier, out output);

        //If it does not exist, throw
        if (output is null) throw new Exception($"Resource {identifier} not found.");

        //If it is not meant to return the cached reference, make a copy of it.
        if (!getCached)
        {
            output = (TRes)output.Duplicate(true);
        }

        return output;
    }

    public static string GetDefaultIdentifier(TRes resource)
    {
        return resource.ResourcePath.GetFile().GetBaseName();
    }

    public void LoadAllInFolder(string path)
    {
        foreach (var item in ResourceLoader.ListDirectory(path))
        {
            TRes res = GD.Load<TRes>(Path.Combine(path + item));
            string identifier = GetDefaultIdentifier(res);

            if (identifier == "") throw new Exception("No identifier could be retrieved");

            AddResource(identifier, res);
        }
    }
    
}