using System.Threading.Tasks;

namespace Godot;

public class ResourcePack<TRes> where TRes : Resource, new()
{
    public const string DEFAULT_DIR = "Resources";
    public const string INVALID_PACK_IDENTIFIER = "";
    private const string DEFAULT_CATEGORY = "unsorted";
    public const string METAKEY_TAG = "tag";
    public const string METAKEY_IDENTIFIER = "ResPackIdentifier";
    public const string TAG_PERSISTENT = "presistent";

    private Dictionary<string, TRes> Contents = new();
    private Dictionary<string, TRes> ContentsPersistent = new();
    private List<TRes> Pooled = new();
    public bool AutoPoolPersistent = true;
    public readonly string PackIdentifier = "";


    public ResourcePack()
    {
        PackIdentifier = GetPackIdentifier();
        PrepareDirectories();
    }
    public ResourcePack(string packIdentifier)
    {
        PackIdentifier = packIdentifier;
        PrepareDirectories();
    }

    #region Pooled
    public void PooledAdd(TRes res)
    {
        Pooled.Add(res);
    }

    public void PooledRemove(TRes res)
    {
        Pooled.Remove(res);
    }

    public List<TRes> PooledGetAll()
        => new(Pooled);

    public List<TRes> PooledGetWithTag(string tag)
        => new(from res in Pooled where TagGet(res).Contains(tag) select res);
    #endregion

    #region Resource
    public void AddResource(string identifier, TRes resource, bool replace = false, bool persistent = false)
    {
        Dictionary<string, TRes> contentsCollection = persistent
            ? ContentsPersistent : Contents;

        if (contentsCollection.ContainsKey(identifier) && !replace)
        {
            throw new Exception("Resource already exists.");
        }

        contentsCollection[identifier] = resource;

        _Loaded(identifier, resource);
    }

    public void RemoveResource(string identifier, bool persistent = false)
    {
        Dictionary<string, TRes> contentsCollection = persistent
            ? ContentsPersistent : Contents;
        contentsCollection.Remove(identifier);
    }

    public bool HasResource(string identifier, bool persistent = false)
    {
        Dictionary<string, TRes> contentsCollection = persistent
            ? ContentsPersistent : Contents;

        return contentsCollection.ContainsKey(identifier);
    }

    public TRes ResourceGet(string identifier, bool persistent = false, bool contentsFallback = true)
    {
        TRes? output;

        Dictionary<string, TRes> contentsCollection = persistent
            ? ContentsPersistent : Contents;
        contentsCollection.TryGetValue(identifier, out output);

        if (output is null && contentsFallback) Contents.TryGetValue(identifier, out output);
        //If it does not exist, throw
        if (output is null) throw new Exception($"Resource {identifier} not found.");

        //Make a copy if it is not persistent, otherwise just keep modifying it.
        output = (TRes)output.Duplicate(true);

        return output;
    }
    public List<TRes> ResourcesGetWithTag(string tag, bool persistent = false)
        => (from identifier
            in persistent ? Contents.Keys : ContentsPersistent.Keys
            select ResourceGet(identifier, persistent)
            ).ToList();

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
    #endregion

    #region Tag
    public static void TagAdd(TRes res, string tag)
    {
        Collections.Array<string> tags = TagGet(res);
        if (!tags.Contains(tag)) tags.Add(tag);
        TagSet(res, tags);
    }

    public void TagRemove(string identifier, string tag)
    {
        if (Contents.ContainsKey(identifier))
            throw new Exception("No resource with that identifier was found");

        Collections.Array<string> tags = TagGet(identifier);
        tags.Remove(tag);
        TagSet(identifier, tags);
    }

    public static void TagSet(TRes res, Collections.Array<string> tags)
        => res.SetMeta(METAKEY_TAG, tags);
    protected void TagSet(string identifier, Godot.Collections.Array<string> tags)
        => TagSet(Contents[identifier], tags);

    public void TagClear(string identifier)
    {
        TagSet(identifier, []);
    }

    public static bool TagIn(TRes res, string tag)
        => TagGet(res).Contains(tag);

    public static Collections.Array<string> TagGet(TRes res)
        => res.GetMeta(METAKEY_TAG, new Collections.Array<string>())
            .As<Collections.Array<string>>();
    public Collections.Array<string> TagGet(string identifier)
        => TagGet(Contents[identifier]);

    [Obsolete("The saved resource is kinda fucked.")]
    protected void TagFormatRes(string path, TRes res)
    {
        //Check if it is possible to save with this extension.
        //if (!ResourceSaver.GetRecognizedExtensions(res).Contains(path.GetExtension()))
        //Only modify resources that end in .tres
        if (path.GetExtension() != "tres") return;

        if (!TagIsFormatted(res))
        {
            TagSet(res, []);
            var error = ResourceSaver.Save(res, path);
            if (error != Error.Ok) throw new Exception($"Failed to save with error {error}");
        }
    }

    protected static bool TagIsFormatted(TRes res)
        => res.HasMeta(METAKEY_TAG)
        && res.GetMeta(METAKEY_TAG).As<Collections.Array<string>>() is Collections.Array<string>;

    #endregion

    #region Load
    public void LoadContent(bool persistent = false, bool preClear = true)
    {
        //If it is going to replace everything, just go ahead.
        if (persistent)
        {
            if (preClear)
            {
                ContentsPersistent = LoadGetAllInFolder(GetDirectory(persistent));
                return;
            }

            //Otherwise add the new elements.
            foreach (var item in LoadGetAllInFolder(GetDirectory(persistent)))
            {
                ContentsPersistent.Add(item.Key, item.Value);
            }
        }
        else
        {
            if (preClear)
            {
                Contents = LoadGetAllInFolder(GetDirectory(persistent));
                return;
            }

            //Otherwise add the new elements.
            foreach (var item in LoadGetAllInFolder(GetDirectory(persistent)))
            {
                Contents.Add(item.Key, item.Value);
            }
            CreateEnums();
        }
    }

    public Dictionary<string, TRes> LoadGetAllInFolder(string path)
    {
        Dictionary<string, TRes> output = new();
        foreach (var item in ResourceLoader.ListDirectory(path))
        {
            string loadPath = path + "/" + item;
            TRes res = GD.Load<TRes>(loadPath);
            string identifier = GetResourceIdentifier(res);

            if (identifier == "") throw new Exception("No identifier could be retrieved");

            output.Add(identifier, res);
        }
        return output;
    }

    public virtual void _Loaded(string identifier, TRes res)
    {

    }
    #endregion

    #region Save
    public void SavePersistent()
    {
        IEnumerable<TRes> toSave =
            ContentsPersistent.Values.Where(x => TagIn(x, TAG_PERSISTENT))
            .Concat(PooledGetWithTag(TAG_PERSISTENT));

        foreach (var item in toSave)
        {
            string savePath = GetResourceSavePath(item);
            var error = ResourceSaver.Save(
                item,
                savePath
                );
            if (error != Error.Ok) throw new Exception($"Cannot save {item} in path {savePath} due to error {error}");
        }
    }
    #endregion

    #region Files
    private string GetDefaultResourcePath()
        => $"{GetDirectory(false)}/Default{GetExtension()}";

    public bool DefaultResourceExists() => FileAccess.FileExists(GetDefaultResourcePath());

    protected void PrepareDirectories()
    {
        string userDir = GetDirectory(true);
        string resDir = GetDirectory(false);
        var userErr = DirAccess.MakeDirRecursiveAbsolute(userDir);
        var resErr = DirAccess.MakeDirRecursiveAbsolute(resDir);

        if (userErr != Error.Ok)
            throw new Exception($"Failed to make directories.\nUser DIR | ERROR: {userDir} | {userErr}\nRes DIR | ERROR: {resDir} | {resErr}");
    }

    public void CreateDefault()
    {

        if (DefaultResourceExists()) goto verify;

        //Create a new one if there is not even a file there.
        if (OS.HasFeature("editor"))
        {
            string path = $"{GetDirectory(false)}/Default{GetExtension()}";
            Error result = ResourceSaver.Save(new TRes(), path);
            GD.PushError($"Resource creation finished with code '{result}' at path '{path}' of category '{GetPackIdentifier()}");
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
        if (!OS.HasFeature("editor")) return;

        string enumName = $"EPackID{GetPackIdentifier()}";

        DirAccess.MakeDirRecursiveAbsolute($"{GetBaseDirectory(false)}/ENUMS");

        FileAccess file = FileAccess.Open(
            $"{GetBaseDirectory(false)}/ENUMS/{enumName}.cs",
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
            return $"user://Save/{DEFAULT_DIR}/";
        }
        else
        {
            return $"res://{DEFAULT_DIR}/";
        }
    }

    public string GetDirectory(bool user)
    {
        string uniqueString = GetPackIdentifier();
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

    public string GetPackIdentifier()
    {
        //If an override was set, use that.
        if (PackIdentifier != INVALID_PACK_IDENTIFIER) return PackIdentifier;

        //Try to infer it from the type extension
        string output = typeof(TRes).ToString().GetExtension();

        //If empty, get the original as a backup.
        if (output == "") throw new Exception("Could not get an identifier.");//output = typeof(TRes).ToString();

        return output;
    }

    public virtual string GetResourceIdentifier(TRes resource)
    {
        string output = resource.ResourcePath.GetFile().GetBaseName();
        if (output == "") output = resource.GetMeta(METAKEY_IDENTIFIER, "").As<string>();
        return output;
    }

    public string GetResourceSavePath(TRes resource)
        => $"{GetDirectory(true)}/{GetResourceIdentifier(resource)}{GetExtension()}";
    #endregion
}
public static class ResourcePackExtension
{
    public static void MakePersistent<TRes>(this TRes res) where TRes : Resource, new()
        => ResourcePack<TRes>.TagAdd(res, ResourcePack<TRes>.TAG_PERSISTENT);
}