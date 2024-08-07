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

namespace ChessLike.Shared;

public static class Serializer
{
    public static void SaveAsXml(object obj, string file_path)
    {
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
