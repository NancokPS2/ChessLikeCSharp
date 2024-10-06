using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;
using Godot;

namespace ChessLike.Entity;

public partial class Faction : IResourceSerialize<Faction, FactionResource>
{
    public FactionResource ToResource()
    {
        FactionResource output = new();
        output.Identifier = Identifier;
        output.Inventory = Inventory.ToResource();
        foreach (var item in RelationList)
        {
            output.RelationList.Add(item.Key, item.Value);
        }
        return output;
    }

    public static Faction FromResource(FactionResource resource)
    {
        Faction output = new();
        output.Identifier = resource.Identifier;
        output.Inventory = Inventory.FromResource(resource.Inventory);
        foreach (var item in resource.RelationList)
        {
            output.RelationList.Add(item.Key, item.Value);
        }

        return output;
    }
}
