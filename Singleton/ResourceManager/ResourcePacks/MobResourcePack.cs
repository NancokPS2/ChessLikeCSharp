using System;
using ChessLike.Entity;
using Godot;

public class MobResourcePack : ResourcePack<Mob>
{
    public Mob GetResource(EPackIDMob enu)
    {
        string? output = Enum.GetName<EPackIDMob>(enu);
        return ResourceGet(output ?? throw new Exception("Could not get name from enum."));
    }

    [Obsolete("This is not safe enough.")]
    public override string GetResourceIdentifier(Mob resource)
    {
        string output = base.GetResourceIdentifier(resource);
        if (output == "") output = resource.DisplayedName;
        return output;
    }

}