using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Xml;
using ChessLike.Entity;
using Godot;

namespace ChessLike.Shared;
public partial class StatSet<[MustBeVariant]TStatEnum> : Resource where TStatEnum : notnull, Enum
{
    public readonly TStatEnum[] AllStats;
    public readonly TStatEnum INVALID_STAT_ENUM;

    public const string INVALID_BOOST_SOURCE = "__INVALID__";

    public Dictionary<TStatEnum, float> StatDict { get; set; } = new();
    public Dictionary<string, StatBoost<TStatEnum>> Boosts = new();

    public StatSet(TStatEnum invalidStat)
    {
        INVALID_STAT_ENUM = invalidStat;

        AllStats = (
            from TStatEnum e
            in Enum.GetValues(typeof(TStatEnum))
            where IsValidStat(e)
            select e
            ).ToArray();

        foreach (TStatEnum stat in AllStats)
        {
            StatDict[stat] = new();
        }
    }

    public virtual bool IsValidStat(TStatEnum stat) => true;

    public void ChangeMax(TStatEnum stat, float value)
    {
        SetStat(stat, GetStat(stat) + value);
    }

    public void SetStat(TStatEnum stat, float value)
    {
        StatDict[stat] = value;
    }

    public float GetStat(TStatEnum stat)
    {
        float val;
        StatDict.TryGetValue(stat, out val);
        float additive = 0;
        float multiplicative = 1;
        foreach (var item in Boosts.Values)
        {
            additive += item.GetAdditiveMax(stat);
            multiplicative *= item.GetMultiplicativeMax(stat);
        }
        Debug.Assert(multiplicative != 0);
        return Mathf.Snapped((val + additive) * multiplicative, 0.1f);
    }

    #region Boosts
    public string BoostGetListOfStatChanges(TStatEnum name)
    {
		string baseVal = StatDict[name] == float.MaxValue ? "UNLIMITED" : StatDict[name].ToString();
        string output = $"{name} Boost \nBase: {baseVal}\n";

        foreach (StatBoost<TStatEnum> boost in Boosts.Values)
        {
            string source = boost.Source;
            float total_mult = boost.GetMultiplicativeMax(name);
            float total_add = boost.GetAdditiveMax(name);
            output += $"{source}: +{total_add} | (x{total_mult})\n";
        }
        return output;
    }

    public string BoostGetListOfStatChanges()
    {
        string output = "";
        foreach (var item in AllStats)
        {
            output += BoostGetListOfStatChanges(item) + "\n";
        }
        return output;
    }

    private StatBoost<TStatEnum> BoostGetFromSource(string source)
    {
        if (Boosts.ContainsKey(source))
        {
            return Boosts[source];
        }
        else
        {
            return new(source);
        }
    }

    public void BoostAdd(IStatBooster booster, bool replace)
    {
        if (booster.GetBoostSource() == INVALID_BOOST_SOURCE) { throw new Exception("Invalid source."); }
        StatBoost<TStatEnum>? boost = booster.GetStatBoost();
        if (boost is not null)
        {
            BoostAdd(boost);
        }
    }

    public void BoostAdd(StatBoost<TStatEnum> boost, bool replace = true)
    {
        string source = boost.Source;
        //Replacing the boost with a new one. Force a replacement if there is no source in the first place.
        if (replace || !Boosts.ContainsKey(source))
        {
            Boosts[source] = boost;
        }
        //Not replacing and there is an existing boost, add to it.
        else
        {
            Boosts[source] = StatBoost<TStatEnum>.Combined(Boosts[source], boost);
        }

    }


    public void BoostRemove(string source)
    {
        if (source == INVALID_BOOST_SOURCE) { throw new Exception("Invalid source."); }
        if (Boosts.ContainsKey(source))
        {
            Boosts.Remove(source);
        }
    }

    #endregion

    public static TStatEnum[] GetArrayOfNames()
    {
        int total_stats = Enum.GetNames(typeof(TStatEnum)).Length;
        TStatEnum[] output = new TStatEnum[total_stats];

        int index = 0;
        foreach (TStatEnum name in Enum.GetValues(typeof(TStatEnum)))
        {
            output[index] = name;
            index++;
        }
        return output;
    }


    public interface IStatBooster
    {
        public string GetBoostSource();
        public StatBoost<TStatEnum>? GetStatBoost();
    }
}

