using System;
using System.Xml;
using ChessLike.Entity;

namespace ChessLike.Shared;

/// <summary>
/// Loads and saves objects from and to disk.
/// Any new object type that needs to be loaded must be defined in the Key enum.
/// Use SaveObject() and LoadObject() to load specific files. 
/// "identifier"s are the file name without their extension.
/// </summary>
public class ResourceDictionary
{

    public const string FileExtension = ".xml";
    public string current_user = "DEFAULT";

    public enum FileSource
    {
        USER_CONTENT,//Appdata\Local
        GAME_CONTENT,//Game directory
    }

    public enum Key
    {
        UNKNOWN,
        ENTITY,
        ACTION,
        FACTION,
        ITEM,
    }

    Dictionary<Key, Dictionary<string, Object>> contents;

    public ResourceDictionary()
    {
        contents = new();
        foreach (Key key in Enum.GetValues(typeof(Key)))
        {
            contents[key] = new();
        }
    }

    public void Add(Key key, string identifier, Object obj)
    {
        contents[key][identifier] = obj;
    }

    public void Add(string identifier, Object obj)
    {
        Key key = GetObjectKey(obj);
        Add(key, identifier, obj);
    }

    public bool Remove(Key key, string identifier)
    {
        return contents[key].Remove(identifier);

    }

    public bool Remove(string identifier, Object obj)
    {
        Key key = GetObjectKey(obj);
        return Remove(key, identifier);
    }

    public bool Exists(Key key, string identifier)
    {
        Object? tried = new();
        return contents[key].TryGetValue(identifier, out tried);
    }

    public void SaveObject(string identifier, Object obj, FileSource source = FileSource.USER_CONTENT)
    {
        Key key = GetObjectKey(obj);
        string path = new(GetResourceDirectory(source, key) + @"\" + GetFormattedFileName(identifier));
        //TODO
        //SaveObjectToXml(GetPath(file_name, source, true), obj);
    }

    public Object? LoadObject(string identifier, Key key, FileSource source = FileSource.USER_CONTENT)
    {
        Type type = GetKeyType(key);
        string path = new(GetResourceDirectory(source, key) + @"\" + GetFormattedFileName(identifier));
        //TODO
        return this;
    }

    public static string GetResourceDirectory(FileSource source, Key key, bool ensure_exists = true)
    {

        if (key == Key.UNKNOWN)
        {
            throw new ArgumentException("Key cannot be UNKNOWN.");
        }

        string dir;
        switch (source)
        {
            case FileSource.USER_CONTENT: 
                dir = Global.Directory.UserContent;
                break;
            
            case FileSource.GAME_CONTENT:
                dir = Global.Directory.GameContent;
                break;

            default:
                throw new NullReferenceException("The hell?");
        }

        #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string key_name = Enum.GetName(typeof(Key), key);

        #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        dir = new(dir + @"\" + key_name);

        if(ensure_exists)
        {   
            Directory.CreateDirectory(dir);
        }

        return dir;
    }

    public static string GetFormattedFileName(string file_name)
    {
        string extension = Path.GetExtension(file_name);

        if(extension == FileExtension)
        {
            return file_name;
        }

        if(extension == "")
        {
            return new(file_name + FileExtension);
        }else
        {
            throw new ArgumentException("This file name already has an extension but it is invalid.");
        }
    }

    public static List<string> GetResourcePaths(FileSource source, Key key)
    {
        string dir = GetResourceDirectory(source, key);
        string[] files = Directory.GetFiles(dir);
        List<string> output = new();
        
        foreach (string path in files)
        {
            //Must end with the designated extension.
            if (Path.GetExtension(path) != FileExtension)
            {
                continue;
            }

            output.Add(path);
            
            /*
            string file_name = Path.GetFileNameWithoutExtension(path);
            Object? loaded = LoadObjectFromXml(path, GetKeyType(key));
            if(loaded == null)
            {
                throw new NullReferenceException("Could not load file!");
            } 
            */
        }
        return output;
    }

    public static Key GetObjectKey(Object obj)
    {
        if (obj is Entity.Mob)
        {
            return Key.ENTITY;
        }

        if (obj is Faction)
        {
            return Key.FACTION;
        }

        if (obj is Entity.Action)
        {
            return Key.ACTION;
        }

        if (obj is Storage.Item)
        {
            return Key.ITEM;
        }

        throw new ArgumentException("No Key could be found for this object.");
    }

    public static Type GetKeyType(Key key)
    {
        switch (key)
        {
            case Key.ENTITY:
                return typeof(Mob);

            case Key.FACTION:
                return typeof(Faction);

            case Key.ACTION:
                return typeof(Entity.Action);

            case Key.ITEM:
                return typeof(Storage.Item);

            case Key.UNKNOWN:
                throw new ArgumentException("No type exists for this key.");

            default:
                throw new ArgumentException("Invalid key");
        }
    }

    /// <summary>
    /// Loads files of the given keys, overwriting existing files.
    /// </summary>
    /// <param name="keys">Which types of objects to load. If empty, uses all keys.</param>

    public interface IResource
    {
        public Object? FromResource<T>() where T : class;
    }



}
