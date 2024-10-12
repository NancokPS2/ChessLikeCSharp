using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Shared.Storage;
using Godot;

namespace ChessLike.Entity;

public partial class Mob : IResourceSerialize<Mob, MobResource>
{   

    public MobResource ToResource()
    {
        MobResource output = new();

        output.DisplayedName = DisplayedName;
        output.Jobs.AddRange(from job in Jobs select job.Identifier);
        //output.Actions.AddRange(from action in Actions select action.ToResource());
        output.Race = Race;
        output.Faction = Faction;
        output.MobInventory = MobInventory.ToResource();

        return output;
    }

    public static Mob FromResource(MobResource res)
    {
        Mob output = new();
        output.DisplayedName = res.DisplayedName;
        output.Race = res.Race;
        output.Faction = res.Faction;
        output.MobInventory = (Inventory)Inventory.FromResource(res.MobInventory);
        output.Jobs.AddRange(from identifier in res.Jobs select Global.ManagerJob.GetFromEnum(identifier));

        return output;
    }

}
