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
		SetAssociatedStat(EValueName.REACTION, EStatName.REACTION);
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
    
