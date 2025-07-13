using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Extension;
using ChessLike.Storage;
using Godot;
using Godot.Collections;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobTemplate : Resource
{
    public enum ETemplateType {JOB, RACE, BASE}

    [Export]
    protected ETemplateType Type;

    [Export]
    protected Array<string> Names = new();
    /* [Export]
    private Godot.Collections.Array<string> names
    {
        set => Names = new(names);
        get => new(Names);
    } */

    [Export]
    protected Array<Ability> Abilities = new();

    [Export]
    protected Array<Item> Equipment = new();

    [Export]
    protected MobStatSet? MobStatsBase = Mob.GetDefaultStats();

    [Export]
    protected Array<MobStatBoost> StatBoosts = new();

    [Export]
    protected Array<ERace> Races = new();

    /// <summary>
    /// Applies the template to a mob, any non empty fields of the template will replace parts of the mob.
    /// </summary>
    /// <param name="mob"></param>
    /// <returns></returns>
    public Mob ApplyTemplate(Mob mob)
    {
        //Name
        mob.DisplayedName = Names.GetRandom(mob.DisplayedName);

        //Abilities
        foreach (var item in Abilities)
        {
            mob.AddAction(item);
        }

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
        mob.Stats.Refill();

        //StatBoosts
        string boostSource = Type.ToString();
        MobStatBoost finalBoost = new(boostSource);
        foreach (var item in StatBoosts)
        {
            item.Source = boostSource;
            finalBoost += item;
        }
        mob.Stats.BoostAdd(finalBoost, true);

        //Race
        mob.Race = Races.GetRandom(mob.Race);
        return mob;
    }
}
