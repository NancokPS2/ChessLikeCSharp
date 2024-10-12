using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtendedXmlSerializer.ExtensionModel.Content;

namespace ChessLike.Entity;

public partial class MobStatSet : StatSet<StatName>, IResourceSerialize<MobStatSet, MobStatSetResource>
{
    public MobStatSet() : base()
    {

    }

    public  MobStatSet(StatSet<StatName> stats) : this()
    {
        Contents = stats.Contents;
    }
    public new MobStatSetResource ToResource()
    {
        MobStatSetResource output = new();
        foreach (var item in Contents)
        {
            output.Contents.Add(item.Key, item.Value.GetMax());
        }
        return output;
    }

    public static MobStatSet FromResource(MobStatSetResource resource)
    {
        MobStatSet output = new MobStatSet();
        
        List<StatName> test1 = resource.Contents.Keys.ToList<StatName>();
        List<StatName> test2 = output.Contents.Keys.ToList<StatName>();
        Array test3 = Enum.GetValues(typeof(StatName));
        foreach (var item in resource.Contents)
        {
            output.SetStat(item.Key, item.Value);
        }

        return output;
    }
}
    
