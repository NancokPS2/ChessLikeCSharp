using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Shared;

public interface IResourceSerialize<TSerialized, TResource> where TResource : Godot.Resource
{
    public TSerialized FromResource(TResource resource);

    public TResource ToResource();

    public string GetResourceFolderRes() => "res://Resources/" + this.GetType().ToString();

    public string GetResourceFolderUser() => "user://Resources/" + this.GetType().ToString();
}
