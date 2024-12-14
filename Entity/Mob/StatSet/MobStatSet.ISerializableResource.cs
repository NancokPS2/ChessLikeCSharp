using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ChessLike.Entity;

public partial class MobStatSet : StatSet<StatName>, IResourceSerialize<MobStatSet, MobStatSetResource>
{
    public MobStatSet() : base()
    {

    }

    public  MobStatSet(StatSet<StatName> stats) : this()
    {
        MaxDict = stats.MaxDict;
    }
    public new MobStatSetResource ToResource()
    {
        MobStatSetResource output = new();
        foreach (var item in MaxDict)
        {
            output.Contents[item.Key] = GetMax(item.Key);
        }
        return output;
    }

    public static MobStatSet FromResource(MobStatSetResource resource)
    {
        MobStatSet output = new MobStatSet();
        
        foreach (var item in resource.Contents)
        {
            output.SetStat(item.Key, item.Value);
        }

        return output;
    }

    public override string ToString()
    {
        string output = "";
        foreach (StatName item in MaxDict.Keys)
        {
            output += item.ToString() + ": " 
            + GetValue(item).ToString() + "/" 
            + GetMax(item).ToString() + "\n";
        }
        return output;
    }
}
    
