namespace Godot;

public class ResourcePack<TRes> where TRes : Resource, new()
{
    public const string DEFAULT_DIR = "Resources";
    public const string INVALID_UNIQUE_STRING = "";

    private Dictionary<string, TRes> Contents = new();
    private UniqueList<TRes> Pooled = new();
    public readonly string UniqueString = "";

    public ResourcePack()
    {
        UniqueString = GetUniqueString();
        PrepareDirectories();
    }
    public ResourcePack(string uniqueString)
    {
        UniqueString = uniqueString;
        PrepareDirectories();
    }

    public bool AddPooled(TRes res)
        => Pooled.Add(res);

    public void RemovePooled(TRes res)
        => Pooled.Remove(res);

    public List<TRes> GetAllPooled()
        => Pooled;

    public string GetUniqueString()
    {
        //If an override was set, use that.
        if (UniqueString != INVALID_UNIQUE_STRING) return UniqueString;

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

    public bool HasResource(string identifier)
        => Contents.ContainsKey(identifier);

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

    public void LoadAllInFolder(bool user = false)
        => LoadAllInFolder(GetDirectory(user));

    public void LoadAllInFolder(string path)
    {
        foreach (var item in ResourceLoader.ListDirectory(path))
        {
            TRes res = GD.Load<TRes>(path + "/" + item);
            string identifier = GetDefaultIdentifier(res);

            if (identifier == "") throw new Exception("No identifier could be retrieved");

            AddResource(identifier, res);
        }

        if (OS.HasFeature("editor"))
        {
            CreateEnums();
        }
    }

    protected TRes? GetDefaultResource()
    {
        TRes output;

        string path = GetDefaultResourcePath();

        output = GD.Load<TRes>(path);
        if (output is null)
        {
            return null;
        }

        return (TRes)output.Duplicate(true);
    }

    private string GetDefaultResourcePath()
        => $"{GetDirectory(false)}/Default{GetExtension()}";


    public bool DefaultResourceExists() => FileAccess.FileExists(GetDefaultResourcePath());

    protected void PrepareDirectories()
    {
        DirAccess.MakeDirAbsolute(GetDirectory(true));
        DirAccess.MakeDirAbsolute(GetDirectory(false));
    }

    public void CreateDefault()
    {

        if (DefaultResourceExists()) goto verify;

        //Create a new one if there is not even a file there.
        if (OS.HasFeature("editor"))
        {
            string path = $"{GetDirectory(false)}/Default{GetExtension()}";
            Error result = ResourceSaver.Save(new TRes(), path);
            GD.PushError($"Resource creation finished with code '{result}' at path '{path}' of category '{GetUniqueString()}");
        }
        else
        {
            GD.PushError("Cannot create a default Resource outside an editor build.");
        }

    verify:
        if (GetDefaultResource() is null) throw new Exception("Could not load a default resource.");
    }

    protected void CreateEnums()
    {
        string enumName = $"EPackID{GetUniqueString()}";
        FileAccess file = FileAccess.Open(
            $"{GetBaseDirectory(false)}/{enumName}.cs",
            FileAccess.ModeFlags.WriteRead
            );

        string text =
        $"public enum {enumName} \n"
        + "{\n";
        foreach (var item in Contents)
        {
            text += item.Key + ",\n";
        }
        text += "}";

        file.StoreString(text);
        file.Flush();
        file.Close();
    }

    public string GetBaseDirectory(bool user)
    {
        if (user)
        {
            return $"user://{DEFAULT_DIR}/";
        }
        else
        {
            return $"res://{DEFAULT_DIR}/";
        }
    }

    public string GetDirectory(bool user)
    {
        string uniqueString = GetUniqueString();
        return GetBaseDirectory(user) + uniqueString;
    }

    public virtual string GetExtension()
    {
        if (typeof(TRes) == typeof(PackedScene))
            return ".tscn";
        else if (typeof(TRes) == typeof(FontFile))
            return ".otf";
        else
            return ".tres";
    }
}