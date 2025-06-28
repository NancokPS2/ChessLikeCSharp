using System;
using ChessLike.Entity;
using Godot;

public class JobResourcePack : ResourcePack<Job>
{
    public Job GetResource(EPackIDJob enu)
    {
        string? output = Enum.GetName<EPackIDJob>(enu);
        return GetResource(output ?? throw new Exception("Could not get name from enum."));
    }
}
