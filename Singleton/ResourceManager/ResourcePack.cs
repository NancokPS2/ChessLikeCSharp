using System.Diagnostics;
using System.Threading.Tasks;
using ChessLike.Extension;

namespace Godot;

public class ResourcePack<TRes> : IResourcePack where TRes : Resource, new()
{
    public const string DEFAULT_DIR = "Resources";
    public const string INVALID_PACK_IDENTIFIER = "";
    private const string DEFAULT_CATEGORY = "unsorted";
    public const string METAKEY_TAG = "tag";
    public const string METAKEY_IDENTIFIER = "ResPackIdentifier";
    public const string TAG_PERSISTENT = "persistent";

	protected event EventHandler ContentLoaded;

    protected Dictionary<string, TRes> ContentBase = new();
    protected Dictionary<string, TRes> ContentRuntime = new();
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


	#region Resource
	public void ResourceAddPersistent(string identifier, TRes resource)
	{
		ResourceAdd(identifier, resource, true, true);
	}
    protected void ResourceAdd(string identifier, TRes resource, bool replace = false, bool persistent = false)
	{
		Dictionary<string, TRes> contentsCollection = persistent
			? ContentRuntime : ContentBase;

		if (contentsCollection.ContainsKey(identifier) && !replace)
		{
			throw new Exception("Resource already exists.");
		}

		contentsCollection[identifier] = resource;
	}

    protected void RemoveResource(string identifier, bool persistent = false)
    {
        Dictionary<string, TRes> contentsCollection = persistent
            ? ContentRuntime : ContentBase;
        contentsCollection.Remove(identifier);
    }

	public void ResourceClear(bool onlyPersistent)
	{
		if (onlyPersistent)
			ContentRuntime.Clear();
		else
		{
			ContentBase.Clear();
			ContentRuntime.Clear();
		}
	}

    public bool HasResource(string identifier, bool persistent = false)
	{
		Dictionary<string, TRes> contentsCollection = persistent
			? ContentRuntime : ContentBase;

		return contentsCollection.ContainsKey(identifier);
	}

    public virtual TRes ResourceGet(string identifier, bool getBase = false, bool duplicate = true)
    {
        TRes? output;

		//Select what content to get from.
		Dictionary<string, TRes> contentsCollection;
		if (getBase)
		{
			contentsCollection = ContentBase;
		}
		else
		{
			contentsCollection = ContentRuntime;
		}

        contentsCollection.TryGetValue(identifier, out output);

		if (output is null)
		{
			ContentBase.TryGetValue(identifier, out output);
			if (output is not null)
				MsgLog.LogInfoMsg($"{identifier} resource from pack {PackIdentifier} was not found in runtime content and had to be fetched from base content");
		}

        //If it does not exist, throw
		if (output is null) throw new Exception($"Resource {identifier} not found in either the base or runtime content.");


		return duplicate ? (TRes)output.Duplicate(true) : output;
    }

	public List<TRes> ResourceGetAll(bool persistent)
	{
		if (persistent)
		{
			return ContentRuntime.Values.ToList();
		}
		else
		{
			return ContentBase.Values.ToList();
		}
	}

    public List<TRes> ResourcesGetWithTag(string tag, bool persistent = false)
		=> (from identifier
			in persistent ? ContentBase.Keys : ContentRuntime.Keys
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
        if (ContentBase.ContainsKey(identifier))
            throw new Exception("No resource with that identifier was found");

        Collections.Array<string> tags = TagGet(identifier);
        tags.Remove(tag);
        TagSet(identifier, tags);
    }

    public static void TagSet(TRes res, Collections.Array<string> tags)
        => res.SetMeta(METAKEY_TAG, tags);
    protected void TagSet(string identifier, Godot.Collections.Array<string> tags)
        => TagSet(ContentBase[identifier], tags);

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
        => TagGet(ContentBase[identifier]);

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
	//Load all resources.
	public void LoadContent(bool loadBase, string persistentFolder = "")
	{
		if (!loadBase && persistentFolder == "")
			throw new Exception("Skipping base content AND persistent content, this call can't load anything.");

		if (!loadBase && ContentBase.IsEmpty())
			throw new Exception("The loading of base content was skipped, but no base content has been loaded beforehand.");

		//Start by loading the regular content.
		if (!loadBase)
			goto runtimeContent;

		ResourceClear(false);
		foreach (var item in LoadGetAllInFolder(GetDirectory()))
		{
			ContentBase.Add(item.Key, item.Value);

			//If it is persistent, also put it in said dictionary.
			//This may throw if it was already added in the user section, this should simply not run after loading user content
			//if (item.Value.TagIn(TAG_PERSISTENT)) ContentRuntime.Add(item.Key, item.Value);
			ContentRuntime.Add(item.Key, item.Value);
		}

	runtimeContent:
		//Then the persistent content
		string persistentDir = $"{persistentFolder}/{PackIdentifier}";
		foreach (var item in LoadGetAllInFolder(persistentDir))
		{
			//If this was fetched from the save file folder, it is supposed to be persistent.
			if (!item.Value.TagIn(TAG_PERSISTENT))
				throw new Exception($"Found content ({item.Value}) that is not set as persistent on the save file folder: {persistentDir}.");

			//Content from the save file always overrides existing content
			ContentRuntime[item.Key] = item.Value;
		}

		ContentLoaded?.Invoke(this, new EventArgs());
	}

    public Dictionary<string, TRes> LoadGetAllInFolder(string path)
	{
		Dictionary<string, TRes> output = new();
		foreach (var item in ResourceLoader.ListDirectory(path))
		{
			string loadPath = path + "/" + item;
			TRes res = GD.Load<TRes>(loadPath);
			//If this throws, make sure the load order of the resource packs is correct.
			if (res is null) throw new Exception($"Loaded resource at {loadPath} is null.");
			string identifier = GetResourceIdentifier(res);

			if (identifier == "") throw new Exception("No identifier could be retrieved");

			output.Add(identifier, res);
		}
		return output;
	}
	#endregion

	#region Save
	public bool SavePersistent(string persistentFolder)
	{
		bool success = true;
		List<TRes> toSave =
			ContentRuntime.Values.Where(x => TagIn(x, TAG_PERSISTENT))
			.ToList();

		foreach (var item in toSave)
		{
			string profileName = SaveManager.GetCurrentSave()?.ProfileName ?? throw new Exception();
			int slot = SaveManager.GetCurrentSlot();
			//baseFolder = SaveFile.GetSaveResourceFolder(profileName, slot);
			string savePath = $"{persistentFolder}/{PackIdentifier}/{GetResourceIdentifier(item)}{GetExtension()}";

			string dirPath = savePath.GetBaseDir();
			Error dirError = DirAccess.MakeDirRecursiveAbsolute(dirPath);
			if (dirError != Error.Ok)
				throw new Exception($"Cannot make directory {dirPath} due to error {dirError}");

			var error = ResourceSaver.Save(
				item,
				savePath
				);
				
			if (error != Error.Ok) success = false;
			if (!success)
				throw new Exception($"Cannot save {item} in path {savePath} due to error {error}");
		}
		return success;
    }
    #endregion

    #region Files
    private string GetDefaultResourcePath()
        => $"{GetDirectory()}/Default{GetExtension()}";

    public bool DefaultResourceExists() => GD.Load<TRes>(GetDefaultResourcePath()) is not null;

    protected void PrepareDirectories()
    {
        string userDir = GetDirectory();
        string resDir = GetDirectory();
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
            string path = $"{GetDirectory()}/Default{GetExtension()}";
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

	[Obsolete("This is not a good approach.")]
    protected void CreateEnums()
	{
		if (!OS.HasFeature("editor")) return;

		string enumName = $"EPackID{GetPackIdentifier()}";

		DirAccess.MakeDirRecursiveAbsolute($"{GetDirectory()}/ENUMS");

		FileAccess file = FileAccess.Open(
			$"{GetDirectory()}/ENUMS/{enumName}.cs",
			FileAccess.ModeFlags.WriteRead
			);

		string text =
		$"public enum {enumName} \n"
		+ "{\n";
		foreach (var item in ContentBase)
		{
			text += item.Key + ",\n";
		}
		text += "}";

		file.StoreString(text);
		file.Flush();
		file.Close();
	}

	public string GetDirectory()
	{
		string uniqueString = GetPackIdentifier();
		string output;
		output = $"res://{DEFAULT_DIR}/{uniqueString}";
		DirAccess.MakeDirRecursiveAbsolute(output);
		return output;
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
    #endregion
}

#region Interface
public interface IResourcePack
{
	public void LoadContent(bool loadBase, string persistentFolder = "");

	public bool SavePersistent(string baseFolder);

	public void ResourceClear(bool persistent);

	public void CreateDefault();
}
#endregion

#region Extension
public static class ResourcePackExtension
{
	public static void MakePersistent<TRes>(this TRes res) where TRes : Resource, new()
		=> ResourcePack<TRes>.TagAdd(res, ResourcePack<TRes>.TAG_PERSISTENT);

	public static void TagSet<TRes>(this TRes res, Collections.Array<string> tags) where TRes : Resource, new()
		=> ResourcePack<TRes>.TagSet(res, tags);

	public static void TagAdd<TRes>(this TRes res, string tag) where TRes : Resource, new()
		=> ResourcePack<TRes>.TagAdd(res, tag);

	public static Collections.Array<string> TagGet<TRes>(this TRes res) where TRes : Resource, new()
		=> ResourcePack<TRes>.TagGet(res);

	public static bool TagIn<TRes>(this TRes res, string tag) where TRes : Resource, new()
		=> ResourcePack<TRes>.TagIn(res, tag);
}
#endregion