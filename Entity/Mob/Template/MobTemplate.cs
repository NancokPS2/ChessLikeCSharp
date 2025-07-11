using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Storage;
using Godot;
using Godot.Collections;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplate : Resource
{
    [Export]
    public string PresetName = "UNNAMED PRESET";

    public Array<string> Names = new();
    /* [Export]
    private Godot.Collections.Array<string> names
    {
        set => Names = new(names);
        get => new(Names);
    } */

    public Array<ItemEquipment> Weapons;

    public Array<string> MobStats;

    public Mob ApplyTemplate(Mob mob)
    {
        mob.DisplayedName = Names.GetRandom(mob.DisplayedName);



        return mob;
    }
}
