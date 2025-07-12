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

    [Export]
    public Array<string> Names = new();
    /* [Export]
    private Godot.Collections.Array<string> names
    {
        set => Names = new(names);
        get => new(Names);
    } */

    [Export]
    public Array<Item> Equipment = new();

    [Export]
    public MobStatSet? MobStatsBase = Mob.GetDefaultStats();

    [Export]
    public Array<ERace> Races = new();

    /// <summary>
    /// Applies the template to a mob, any non empty fields of the template will replace parts of the mob.
    /// </summary>
    /// <param name="mob"></param>
    /// <returns></returns>
    public Mob ApplyTemplate(Mob mob)
    {
        //Name
        mob.DisplayedName = Names.GetRandom(mob.DisplayedName);

        //Equipment
        foreach (var slot in Enum.GetValues<MobEquipmentInventory.ESlot>())
        {
            var candidates = Equipment.Where(
                x => mob.EquipmentInventory.IsValidForSlot(x, slot)
                );

            if (candidates.Count() == 0) continue;

            mob.EquipmentInventory.EquipItem(candidates.GetRandom(), slot, true);
        }

        //Stats
        mob.Stats = MobStatsBase ?? mob.Stats;

        //Race
        mob.Race = Races.GetRandom(mob.Race);
        return mob;
    }
}
