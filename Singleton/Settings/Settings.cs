using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExtendedXmlSerializer;
using Godot;

public partial class Settings : Node
{
    public delegate void FloatChange(KeyFloat key);
    public delegate void StringChange(KeyString key);
    public event FloatChange? FloatChanged;
    public event StringChange? StringChanged;

    const string DEFAULT_PATH = "user://settings.ini";
    const string FLOAT_KEY = "Floats";
    const string STRING_KEY = "Strings";
    public enum KeyFloat {
        VOLUME,
        PARTICLE_RATIO        
    }
    public enum KeyString {
        LOCALE,
    }
    public static Settings Instance;
    private Dictionary<KeyFloat, float> Floats = new();
    private Dictionary<KeyString, string> Strings = new();

    //private Dictionary<, int> Ints = new();

    public Settings()
    {
    }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        Reset();
        SaveSettings();
    }

    public static void Reset()
    {
        Set(KeyFloat.VOLUME, 1f);
        Set(KeyFloat.PARTICLE_RATIO, 1f);
        Set(KeyString.LOCALE, "en");
    }

    public static void Set(KeyFloat key, float val)
    {
        Instance.Floats[key] = val;
        Instance.FloatChanged?.Invoke(key);
    }
    public static void Set(KeyString key, string val)
    {
        Instance.Strings[key] = val;
        Instance.StringChanged?.Invoke(key);
    }

    public static float Get(KeyFloat key)
    {
        if (!Instance.Floats.ContainsKey(key))
        {
            throw new Exception("Unset settings. Missing default value.");
        }
        return Instance.Floats[key];
    }

    public static string Get(KeyString key)
    {
        if (!Instance.Strings.ContainsKey(key))
        {
            throw new Exception("Unset settings. Missing default value.");
        }
        return Instance.Strings[key];
    }

    public static void SaveSettings(string path = DEFAULT_PATH)
    {
        ConfigFile config = new();
        foreach (var item in Instance.Floats)
        {
            config.SetValue(FLOAT_KEY, nameof(item.Key), item.Value);
        }
        foreach (var item in Instance.Strings)
        {
            config.SetValue(STRING_KEY, nameof(item.Key), item.Value);
        }
        config.Save(path);
    }

    public static void LoadSettings(string path = DEFAULT_PATH )
    {
        ConfigFile config = new();
        Error error = config.Load(path);
        if (error != Error.Ok)
        {
            GD.PushWarning("Could not load a file. Creating a new one at path: " + path);
            SaveSettings(path);
            config.Load(path);
        }

        foreach (var item in Enum.GetValues<KeyFloat>())
        {
            if(!config.HasSectionKey(FLOAT_KEY, nameof(item))){throw new Exception("Missing entry.");}
            float val = (float)config.GetValue(FLOAT_KEY, nameof(item), 1f);
            Set(item, val);
        }
        foreach (var item in Enum.GetValues<KeyString>())
        {
            if(!config.HasSectionKey(STRING_KEY, nameof(item))){throw new Exception("Missing entry.");}
            string val = (string)config.GetValue(STRING_KEY, nameof(item), "UNDEFINED");
            Set(item, val);
        }
    }
}
