using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ChessLike.Entity;

public partial class MobStatSet : StatSet<EStatName>
{
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
        foreach (EStatName item in MaxDict.Keys)
        {
            output += item.ToString() + ": " 
            + GetValue(item).ToString() + "/" 
            + GetMax(item).ToString() + "\n";
        }
        return output;
    }
}
    
