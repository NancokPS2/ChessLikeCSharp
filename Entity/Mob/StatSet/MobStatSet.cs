using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Shared.GenericStruct.StatSet;
using ExtendedXmlSerializer.ExtensionModel.Content;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class MobStatSet : StatSetWithValues<EStatName, EValueName>
{
    [Export]
    private Godot.Collections.Dictionary<EStatName, float> statDict
    {
        set => StatDict = new(value);
        get => new(StatDict);
    }

    [Export]
    private Godot.Collections.Dictionary<EValueName, float> valueDict
    {
        set => ValueDict = new(value);
        get => new(ValueDict);
    }

	public MobStatSet() : base(EStatName.NONE, EValueName.NONE)
    {
        SetAssociatedStat(EValueName.HEALTH, EStatName.HEALTH);
        SetAssociatedStat(EValueName.ENERGY, EStatName.ENERGY);
        SetAssociatedStat(EValueName.ACTION, EStatName.ACTION);
        SetAssociatedStat(EValueName.SUB_ACTION, EStatName.SUB_ACTION);
        SetAssociatedStat(EValueName.REACTION, EStatName.REACTION);
        SetAssociatedStat(EValueName.MOVE, EStatName.MOVE);
    }

    public static MobStatSet GetDefault()
    {
        MobStatSet output = new();
        output.SetStat(EStatName.HEALTH, 100);
        output.SetStat(EStatName.ENERGY, 30);
        output.SetStat(EStatName.AGILITY, 100);
        output.SetStat(EStatName.STRENGTH, 100);
        output.SetStat(EStatName.INTELLIGENCE, 100);
        output.SetStat(EStatName.MOVEMENT, 3);
        output.SetStat(EStatName.JUMP, 2);
        output.SetStat(EStatName.DELAY, 100);

		output.SetStat(EStatName.ACTION, 1);
		output.SetStat(EStatName.SUB_ACTION, 1);
		output.SetStat(EStatName.REACTION, float.MaxValue);
		output.SetStat(EStatName.MOVE, 1);
        output.RefillValues();
        return output;
    }

    public override bool IsValidStat(EStatName stat)
        => stat != EStatName.NONE;

    public override bool IsValidValue(EValueName val)
        => val != EValueName.NONE;
    public override string ToString()
    {
        string output = "";
        foreach (EStatName item in AllStats)
        {
            output += item.ToString() + ": "
            + "WIP" + "/"
            + GetStat(item).ToString() + "\n";
        }
        return output;
    }
}
    
