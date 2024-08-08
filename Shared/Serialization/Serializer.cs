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
using ExtendedXmlSerializer.ContentModel.Identification;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Shared;

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

    public static string GetFilePath(IIdentify identify, Global.Directory.Content global_dir)
    {
        string file_name = identify.Identity.Identifier + ".xml";
        string sub_folder = identify.GetType().ToString();
        string path = Path.Combine(Global.Directory.GetContentDir(global_dir), sub_folder, file_name);
        if (!IsFilePathValid(path))
        {
            throw new InvalidDataException("The result was invalid.");
        }
        return path;
    }

    public static void SaveAsXml(IIdentify identity, Global.Directory.Content global_dir)
    {
        string path = GetFilePath(identity, global_dir);        
        SaveAsXml(identity, path);
    }

    public static void SaveAsXml(object obj, string file_path)
    {
        if(!IsFilePathValid(file_path)) {throw new ArgumentException("Invalid path.");}

        //Overrides if it has the appropiate interface TODO
        if (obj is ISerializeOverride serialize_override)
        {
            XmlAttributeOverrides overrides = new();
            foreach (string blacklisted in serialize_override.GetSerializationBlacklist())
            {
                XmlAttributes attributes = new XmlAttributes(){XmlIgnore = true};
                attributes.XmlElements.Add( new(blacklisted));
                overrides.Add(obj.GetType(), blacklisted, attributes);
            }
        }
        
        
        //Create and use serializer.
        IExtendedXmlSerializer serializer = new ConfigurationContainer()
            .UseAutoFormatting()
            .UseOptimizedNamespaces()
            //.EnableImplicitTyping(obj.GetType())
            .Create();

        //XmlSerializer serializer = new XmlSerializer(obj.GetType());
        string xml_string = serializer.Serialize(new XmlWriterSettings {Indent = true}, obj);

        //Create writer.
        TextWriter string_writer = new StreamWriter(file_path);
        string_writer.Write(xml_string);
        string_writer.Close();

    }

    public static T LoadAsXml<T>(string file_path)
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

    public interface ISerializeOverride
    {
        public string[] GetSerializationBlacklist();
    }

}
