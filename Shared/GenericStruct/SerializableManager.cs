using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;
using Godot;

namespace ChessLike.Shared.GenericStruct;

public class SerializableManager<TManaged, TResource> 
    where TManaged : ISerializable, IResourceSerialize<TManaged, TResource> 
    where TResource : Godot.Resource
{
    protected static UniqueList<TManaged> Contents = new();

    public SerializableManager()
    {
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderRes());
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderUser());
        DirectoryInfo? info = Directory.CreateDirectory(GetPrototypeFolder());
        
        //First try to load all existing ones.
        Preload();
        //Then fill any missing ones with prototypes
        SavePrototypes(CreatePrototypes());
    }

    public virtual TManaged ConvertFromResource(TResource resource)
    {
        throw new NotImplementedException();
    }

    public virtual List<TManaged> CreatePrototypes()
    {
        throw new NotImplementedException();
    }
    
    public void Preload()
    {
        
        //Load XML objects.
        List<TManaged>? loaded = Serializer.LoadFolderAsXml<TManaged>(GetPrototypeFolder());
        foreach (var item in loaded)
        {
            Add(item);
        }

        //Load Resources
        string res_debug_output = "";
        foreach (var folder in new string[]{GetResourceFolderRes()+"/", GetResourceFolderUser()+"/"})
        {
            res_debug_output = string.Format("Resources loaded ({0}): ", folder);
            foreach (var item in Godot.DirAccess.GetFilesAt(folder))
            {
                TResource? output;

                string str_to_load = folder + item;
                string extension_to_remove = ".remap";
                int index_to_remove = str_to_load.IndexOf(extension_to_remove);

                if (index_to_remove != -1)
                {
                    str_to_load = str_to_load.Remove(index_to_remove, extension_to_remove.Length);    
                }

                if (str_to_load.EndsWith(".tres") || str_to_load.EndsWith(".res"))
                {
                    output = GD.Load<TResource>(str_to_load);

                    if (output is not null)
                    {
                        Add(ConvertFromResource(output));
                        res_debug_output += "\n " + str_to_load + " | ";
                    }
                }
                else
                {
                    res_debug_output += "\n FAILED: " + str_to_load;
                }

            }
            Debug.Print(res_debug_output);
        }
    }

    public virtual TManaged? LoadPrototype(TManaged managed)
    {
        TManaged? loaded = Serializer.LoadAsXml<TManaged>(Path.Combine(GetPrototypeFolder(), managed.GetFileName() + ".xml"));
        return loaded;

    }
    public virtual string GetPrototypeFolder()
    {
        return Path.Combine( Global.Directory.GetContentDir(EDirectory.USER_CONTENT), "Prototypes");
    }


    public virtual void SavePrototypes(List<TManaged> prototypes)
    {
        
        foreach (TManaged item in prototypes)
        {
            Serializer.SaveAsXml(item, Path.Combine(GetPrototypeFolder(), item.GetFileName() + ".xml"));
        }
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

    public string GetResourceFolderRes() => IResourceSerialize<TManaged, TResource>.GetResourceFolderRes();
    public string GetResourceFolderUser() => IResourceSerialize<TManaged, TResource>.GetResourceFolderUser();
}
