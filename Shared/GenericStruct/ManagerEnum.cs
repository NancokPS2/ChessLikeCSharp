using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Serialization;

namespace ChessLike.Shared.GenericStruct;

public class Manager<TManaged, TEnum> where TManaged : ISerializable
{
    readonly static string PROTOTYPE_FOLDER = Global.Directory.GetContentDir(EDirectory.GAME_CONTENT);
    protected static Dictionary<TEnum, TManaged> Contents = new();

    public Manager()
    {
        SavePrototypes(CreatePrototypes());
        Preload();
    }

    public void Preload()
    {
        Serializer.LoadFolderAsXml<TManaged>(PROTOTYPE_FOLDER);
    }

    public virtual List<TManaged> CreatePrototypes()
    {
        return new();
    }

    public virtual void SavePrototypes(List<TManaged> prototypes)
    {
        
        foreach (TManaged item in prototypes)
        {
            Serializer.SaveAsXml(item, EDirectory.GAME_CONTENT);
        }
    }

    public Dictionary<TEnum, TManaged> GetAll()
    {
        return Contents;
    }

    public void Add(TEnum enum_ident, TManaged managed)
    {
        Contents.Add(enum_ident, managed);
    }

}
