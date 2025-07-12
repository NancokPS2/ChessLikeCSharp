using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobStatSet : StatSet<EStatName>
{

    [Export]
    private Godot.Collections.Dictionary<EStatName, float> maxDict
    {
        set => MaxDict = new(value);
        get => new(MaxDict);
    }

/*
    [Export]
    private Godot.Collections.Dictionary<EStatName, float> currentDict
    {
        set => CurrentDict = new(value);
        get => new(CurrentDict);
    }

    [Export]
    private Godot.Collections.Dictionary<string, MobStatBoost> boosts
    {
        set
        {
            Dictionary<string, StatBoost<EStatName>> input = new();
            foreach (var item in value)
            {
                input[item.Key] = item.Value;
            }
            Boosts = input;
        }
        get
        {
            Godot.Collections.Dictionary<string, MobStatBoost> output = new();
            foreach (var item in Boosts)
            {
                output[item.Key] = new(item.Value);
            }
            return output;
        }

    }
 */
    public MobStatSet() : base()
    {

    }

    protected override bool IsValidStat(EStatName stat)
        => stat != EStatName.NONE;


    public MobStatSet(StatSet<EStatName> stats) : this()
    {
        MaxDict = stats.MaxDict;
    }
    public override string ToString()
    {
        string output = "";
        foreach (EStatName item in AllStats)
        {
            output += item.ToString() + ": "
            + GetValue(item).ToString() + "/"
            + GetMax(item).ToString() + "\n";
        }
        return output;
    }
}
    
