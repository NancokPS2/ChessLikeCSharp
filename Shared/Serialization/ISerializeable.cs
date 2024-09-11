using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ContentModel.Identification;

namespace ChessLike.Shared.Serialization;

//
public interface ISerializable
{
    public string GetFileName();
    public string GetDirectory()
    {
        return Global.Directory.GetContentDir(EDirectory.GAME_CONTENT);
    }

}

public static class ISerializableExtension
{
    public static void SaveAsXml(this ISerializable @this) 
    {
        Serializer.SaveAsXml(@this, @this.GetDirectory());
    }
    public static T? SaveAsXml<T>(this ISerializable @this) where T : ISerializable
    {
        return Serializer.LoadAsXml<T>(@this);
    }
}
