using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Shared;
using ChessLike.Shared.Storage;

namespace ChessLike.Entity;

   
public partial class Job : MobStatSet.IStatBooster
{

    public const string BOOST_SOURCE = "JOB";

    public string GetBoostSource() => BOOST_SOURCE;

    public MobStatSet.StatBoost GetStatBoost()
    {
        MobStatSet.StatBoost output = new(GetBoostSource());
        foreach (var stat in StatMultiplicativeBoostDict)
        {
            StatName stat_name = stat.Key;
            float stat_value = stat.Value;
            output.SetMultiplicativeMax(stat_name, stat_value);
        }

        return output;
    }

}

