using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IResourceSerialize<TSerialized, TResource> where TResource : Godot.Resource
{
    //public TSerialized FromResource(TResource resource);

    public TResource ToResource();

    public static abstract TSerialized FromResource(TResource resource);

    public static string GetResourceFolderRes() => "res://Resources/" + typeof(TSerialized).ToString();

    public static string GetResourceFolderUser() => "user://Resources/" + typeof(TSerialized).ToString();
}
