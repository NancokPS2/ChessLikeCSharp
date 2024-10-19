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
    protected static UniqueList<TManaged> Pooled = new();
    protected static UniqueList<TResource> Resources = new();

    public SerializableManager()
    {
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderRes());
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderUser());
        DirectoryInfo? info = Directory.CreateDirectory(GetPrototypeFolder());
        
        //First try to load all existing ones.
        Preload();
        //DEPRECATED: Then fill any missing ones with prototypes
        //SavePrototypes(CreatePrototypes());
    }

    public virtual TManaged GetFromResource(TResource resource)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// DEPRECATED
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual List<TManaged> CreatePrototypes()
    {
        throw new NotImplementedException();
    }
    
    public void Preload()
    {
        
        //Load XML objects, deprecated.
        List<TManaged>? loaded = Serializer.LoadFolderAsXml<TManaged>(GetPrototypeFolder());
        foreach (var item in loaded)
        {
            AddPooled(item);
        }

        //Load Resources
        string res_debug_output = "";
        foreach (var folder in new string[]{GetResourceFolderRes()+"/", GetResourceFolderUser()+"/"})
        {
            res_debug_output = string.Format("Resources loaded ({0}): ", folder);
            foreach (var item in Godot.DirAccess.GetFilesAt(folder))
            {
                TResource? output;

                //Get the item path.
                string str_to_load = folder + item;

                //Remove the remap extension if present.
                string extension_to_remove = ".remap";
                int index_to_remove = str_to_load.IndexOf(extension_to_remove);
                if (index_to_remove != -1)
                {
                    str_to_load = str_to_load.Remove(index_to_remove, extension_to_remove.Length);    
                }

                //If the filename has a valid extension, proceed.
                if (str_to_load.EndsWith(".tres") || str_to_load.EndsWith(".res"))
                {
                    output = GD.Load<TResource>(str_to_load);

                    if (output is not null)
                    {
                        AddResource(output);
                        //AddPooled(GetFromResource(output));

            //After this, it is all debug printing.
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


    /// <summary>
    /// DEPRECATED
    /// </summary>
    /// <param name="prototypes"></param>
    public virtual void SavePrototypes(List<TManaged> prototypes)
    {
        foreach (TManaged item in prototypes)
        {
            Serializer.SaveAsXml(item, Path.Combine(GetPrototypeFolder(), item.GetFileName() + ".xml"));
        }
    }


    protected List<TManaged> GetAllPooled() => Pooled;

    public void AddPooled(TManaged managed)
    {
        Pooled.Add(managed, false);
    }
    public void AddPooled(List<TManaged> managed) => managed.ForEach(x => AddPooled(x));

    /// <summary>
    /// Gets all loaded resources.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">If this shit's empty, YEET.</exception>
    protected virtual List<TResource> GetAllResources() => Resources.Count != 0 ? Resources : throw new Exception("No resources loaded of type " + typeof(TResource).ToString());
    public void AddResource(TResource resource)
    {
        Resources.Add(resource);
    }
    public void AddResource(List<TResource> resources) => resources.ForEach(x => AddResource(x));

    public string GetResourceFolderRes() => IResourceSerialize<TManaged, TResource>.GetResourceFolderRes();
    public string GetResourceFolderUser() => IResourceSerialize<TManaged, TResource>.GetResourceFolderUser();
}
