using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public struct StatSheet
{
    const float INVALID_FLOAT = float.NaN;
    private Dictionary<string, float> stats = new Dictionary<string, float>();
    private Dictionary<string, Dictionary<string,float>> bonuses = new Dictionary<string, Dictionary<string, float>>();
    private Dictionary<string, Dictionary<string,float>> modifiers = new Dictionary<string, Dictionary<string, float>>();

    public StatSheet(Dictionary<string, float> _stats)
    {
        stats = _stats;
    }

    public void SetStat(string stat_name, float value)
    {
        stats[stat_name] = value;
    }

    public float GetStat(string stat_name, bool ignore_addons = false)
    {
        float output = stats[stat_name];
        foreach (float modifier in GetModifiers(stat_name))
        {
            output *= modifier;
        }
        foreach (float bonus in GetBonuses(stat_name))
        {
            output += bonus;
        }
        return output;
    }

    public void SetBonus(string stat_name, string identifier, float value)
    {
        if(value == INVALID_FLOAT)
        {
            bonuses[stat_name].Remove(identifier);
        }
        bonuses[stat_name][identifier] = value;
    }
    public float[] GetBonuses(string stat_name)
    {
        return bonuses[stat_name].Values.ToArray<float>();
    }

    public void SetModifier(string stat_name, string identifier, float value)
    {
        modifiers[stat_name][identifier] = value;
    }

    public float[] GetModifiers(string stat_name)
    {
        return modifiers[stat_name].Values.ToArray<float>();
    }

}