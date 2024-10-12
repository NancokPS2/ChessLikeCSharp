using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobStatSetResource : Resource
{
    /// <summary>
    /// TEST 2
    /// </summary>
    //[Export] public Godot.Collections.Dictionary<StatName, float> Stats {get => Contents; set => Contents = value;}
    [Export]
    public Godot.Collections.Dictionary<StatName, float> Contents = new();

    public MobStatSetResource()
    {
        foreach (var item in  Mob.GetDefaultStats().Contents)
        {
            Contents[item.Key] = item.Value.GetMax();
        }
    }
    
}
