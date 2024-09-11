using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using ChessLike;
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Shared.Serialization;

public static class Serializer
{

    public static bool IsFilePathValid(string path)
    {
        return path != "" //Must not be empty.
        && path != string.Empty 

        && Path.GetFileName(path) != string.Empty //Must have a file name.
        && Path.GetExtension(path) != string.Empty //Must have an extension.
        && !Path.EndsInDirectorySeparator(path); //If it ends as a directory, it is not a file.
    }

    public static string GetFilePath(ISerializable serializable)
    {
        string file_name = serializable.GetFileName() + ".xml";
        string folder = serializable.GetDirectory();
        string path = Path.Combine(folder, file_name);
        if (!IsFilePathValid(path))
        {
            throw new InvalidDataException(String.Format("The result was invalid. {0}", path));
        }
        return path;
    }

    public static void SaveAsXml(ISerializable serializable, EDirectory global_dir)
    {
        string path = GetFilePath(serializable);        
        SaveAsXml(serializable, path);
    }

    public static void SaveAsXml(object obj, string full_file_path)
    {
        if(!IsFilePathValid(full_file_path)) {throw new ArgumentException("Invalid path.");}

        //Overrides if it has the appropiate interface TODO
/*         string[] whitelist = obj.GetPropertyBlacklist();
        XmlAttributeOverrides overrides = new();
        foreach (string blacklisted in whitelist)
        {
            XmlAttributes attributes = new XmlAttributes(){XmlIgnore = true};
            attributes.XmlElements.Add( new(blacklisted));
            overrides.Add(obj.GetType(), blacklisted, attributes);
        } */
        
        
        //Create and use serializer.
        IExtendedXmlSerializer serializer = new ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            //.EnableImplicitTyping(obj.GetType())
            .Create();

        //XmlSerializer serializer = new XmlSerializer(obj.GetType());
        string xml_string = serializer.Serialize(new XmlWriterSettings {Indent = true}, obj);

        //Ensure the directory exists.
        string dir = Path.GetDirectoryName(full_file_path);
        Directory.CreateDirectory(Path.GetDirectoryName(dir));

        //Create writer.
        TextWriter string_writer = new StreamWriter(full_file_path);
        string_writer.Write(xml_string);
        string_writer.Close();

    }

    public static List<T> LoadFolderAsXml<T>(string folder_path)
    {
        string path_used = Path.Combine(folder_path);
        if (!Directory.Exists(folder_path)){throw new Exception("Not a directory.");}

        List<T> output = new();

        foreach (string item in Directory.EnumerateFiles(folder_path))
        {
            output.Add(LoadAsXml<T>(Path.Combine(folder_path, item)));
        }

        return output;

    }

    public static T? LoadAsXml<T>(ISerializable serializable)
    {
        return LoadAsXml<T>(GetFilePath(serializable));
    }
    public static T? LoadAsXml<T>(string file_path)
    {
        //Get file contents.
        StreamReader stream_reader = new(file_path);

        XmlReaderSettings settings = new XmlReaderSettings{IgnoreWhitespace = false};
        //XmlReader reader = XmlReader.Create(file_path);

        IExtendedXmlSerializer serializer = new ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            .EnableImplicitTyping(typeof(T))
            .Create();
        //XmlSerializer serializer = new(typeof(T));

        T? output = serializer.Deserialize<T>(settings, stream_reader);

        if (output == null)
        {
            throw new NullReferenceException("The object is null.");
        }

        return output;
    }

}
