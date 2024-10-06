using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;

namespace ChessLike.Shared.GenericStruct;

public class SerializableManager<TManaged, TResource> 
    where TManaged : ISerializable, IResourceSerialize<TManaged, TResource> 
    where TResource : Godot.Resource
{
    protected static UniqueList<TManaged> Contents = new();

    public SerializableManager()
    {
        SavePrototypes(CreatePrototypes());
        Preload();
    }

    public virtual List<TManaged> CreatePrototypes()
    {
        throw new NotImplementedException();
    }
    
    public void Preload()
    {
        List<TManaged>? loaded = Serializer.LoadFolderAsXml<TManaged>(GetPrototypeFolder());
        foreach (var item in loaded)
        {
            Add(item);
        }
    }

    public virtual TManaged? LoadPrototype(TManaged managed)
    {
        TManaged? loaded = Serializer.LoadAsXml<TManaged>(Path.Combine(GetPrototypeFolder(), managed.GetFileName() + ".xml"));
        return loaded;

    }
    public virtual string GetPrototypeFolder()
    {
        return Global.Directory.GetContentDir(EDirectory.USER_CONTENT);
    }


    public virtual void SavePrototypes(List<TManaged> prototypes)
    {
        
        foreach (TManaged item in prototypes)
        {
            Serializer.SaveAsXml(item, Path.Combine(GetPrototypeFolder(), item.GetFileName() + ".xml"));
        }
        Godot.DirAccess.MakeDirRecursiveAbsolute(IResourceSerialize<TManaged, TResource>.GetResourceFolderRes());
        Godot.DirAccess.MakeDirRecursiveAbsolute(IResourceSerialize<TManaged, TResource>.GetResourceFolderUser());
    }

    public virtual List<TManaged> GetAll()
    {
        return Contents;
    }

    public void Add(TManaged managed)
    {
        Contents.Add(managed, false);
    }
    public void Add(TManaged[] managed)
    {
        foreach (var item in managed)
        {
            Add(item);
        }
    }

}
