using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared.Serialization;
using Godot;

namespace ChessLike.Shared.GenericStruct;

/// <summary>
/// This class manages objects that can be serialized.
/// </summary>
/// <typeparam name="TManaged">The object to manage, which implements interfaces for serialization trough resources.</typeparam>
/// <typeparam name="TResource">The resource class that this object can be turned into.</typeparam>
public class SerializableManager<TManaged, TResource> 
    where TManaged : ISerializable, IResourceSerialize<TManaged, TResource> 
    where TResource : Godot.Resource
{
    const string META_PROFILENAME = "ProfileName";
    const string META_LAST_UPDATE = "LastUpdate";
    protected string ProfileName = "DEFAULT";
    protected ConfigFile ProfileFile = new();
    protected UniqueList<TManaged> Pooled = new();
    protected UniqueList<TResource> Resources = new();

    public SerializableManager()
    {
        EventBus.ProfileNameChanged += ProfileLoad;
        
        //Create directories.
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderRes());
        Godot.DirAccess.MakeDirRecursiveAbsolute(GetResourceFolderUser());

        Reload();

    }

    protected void Reload()
    {
        Pooled.Clear();
        Resources.Clear();

        //Create all prototypes
        AddResource((from obj in CreatePrototypes() select obj.ToResource()).ToList());
        
        //Load all resources afterwards.
        PreloadResources();

        //Finally load the profile data.
        ProfileLoad(ProfileName);
    }

#region Profile
    public void ProfileLoad(string profile_name)
    {
        ProfileName = profile_name;
        ProfileFile = new();
        Error error = ProfileFile.Load(ProfileGetPath(profile_name));

        //If it could not be loaded.
        if (error != Error.Ok)
        {
            GD.PushError($"Failed to load file with error: {error}");
            ProfileFile = ProfileInitializeFile(profile_name);
        }

        //Confirm the file is valid.
        if (!ProfileIsValid(ProfileFile, profile_name))
        {
            throw new Exception("The file seems to be corrupt.");
        } 
    }

    private bool ProfileIsValid(ConfigFile file, string profile_name)
    {
        bool matching_name = profile_name == (string)file.GetValue("META", META_PROFILENAME, "");
        if(!matching_name){GD.PushError("File has a different profile name from its file name.");}

        return matching_name;
    }

    public void ProfileSave()
    {
        ProfileFile.SetValue("META", META_LAST_UPDATE, Time.GetUnixTimeFromSystem().ToString());
        ProfileFile.Save(ProfileGetPath(ProfileName));
    }

    private ConfigFile ProfileInitializeFile(string profile_name)
    {
        ConfigFile output = new();
        output.SetValue("META", "ProfileName", profile_name);
        output.SetValue("META", "LastUpdate", Time.GetUnixTimeFromSystem().ToString());
        DirAccess.MakeDirRecursiveAbsolute(ProfileGetPath(profile_name).GetBaseDir());
        output.Save(ProfileGetPath(profile_name));
        return output;
    }

    private string ProfileGetPath(string profile_name) => $"user://SaveData/{profile_name}/{GetType()}.cssave";

    public virtual void ProfileSetContent(string key, TResource content)
    {
        ProfileFile.SetValue("CONTENT", key, content);
    }
    public void ProfileSetContent(string key, TManaged managed) => ProfileSetContent(key, managed.ToResource());

    public virtual TResource ProfileGetContent(string key)
    {
        Variant result = ProfileFile.GetValue("CONTENT", key);
        if (result is TResource typed)
        {
            return typed;
        }else
        {
            throw new Exception($"Requested a value of type {typeof(TResource)} but the value was of type {result.GetType()}.");
        }
    }

#endregion

    //TODO
    public virtual List<TManaged> CreatePrototypes()
    {
        return new();
    }

    [Obsolete("Probably don't use this.")]
    public virtual void StorePrototypesAsResources() => CreatePrototypes().ForEach(x => AddResource(x.ToResource()));

#region Pooled
    protected List<TManaged> GetAllPooled() => Pooled;

    public void AddPooled(TManaged managed) => Pooled.Add(managed, false);

    public void AddPooled(List<TManaged> managed) => managed.ForEach(x => AddPooled(x));
#endregion

#region Resources
    /// <summary>
    /// Loads all resources from the designated folders and adds them to the Resource list.
    /// </summary>
    public void PreloadResources()
    {
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

    /// <summary>
    /// Gets all loaded resources.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">If this shit's empty, YEET.</exception>
    protected virtual List<TResource> GetAllResources() => Resources.Count != 0 ? Resources : throw new Exception("No resources loaded of type " + typeof(TResource).ToString());
    public void AddResource(TResource resource) => Resources.Add(resource);
    public void AddResource(List<TResource> resources) => resources.ForEach(x => AddResource(x));
    public string GetResourceFolderRes() => IResourceSerialize<TManaged, TResource>.GetResourceFolderRes();
    public string GetResourceFolderUser() => IResourceSerialize<TManaged, TResource>.GetResourceFolderUser();
#endregion
}
