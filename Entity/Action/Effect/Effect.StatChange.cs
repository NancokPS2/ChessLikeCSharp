namespace ChessLike.Entity.Action;

public class EffectStatChange : Effect
{
    /// <summary>
    /// If true, it will affect the owner.
    /// </summary>
    public bool ChangeMax;
    /// <summary>
    /// Should it affect the current value?
    /// </summary>
    public bool ChangeCurrent;
    /// <summary>
    /// Useful if you want to ADD damage.
    /// </summary>
    public bool InvertResult;

    //Which stat to change.
    public EStatName StatToChange;

    //Which stat of the target to change.
    //Sum this stat from the owner to the value.
    public Dictionary<EStatName, float> StatsAddingFromOwner = new(){
        {EStatName.STRENGTH, 1},
    };

    public Dictionary<EStatName, float> StatsAddingFromTarget = new(){
        {EStatName.STRENGTH, 1},
    };

    public Dictionary<EStatName, float> StatsReducingFromOwner = new(){
        {EStatName.STRENGTH, 1},
    };

    public Dictionary<EStatName, float> StatsReducingFromTarget = new(){
        {EStatName.STRENGTH, 1},
    };

    //How much to change it as a base.
    public float FlatAmount = 0;

    public override void CustomUse(UsageParameters usage_params)
    {
        Mob owner = usage_params.OwnerRef;

        float flat = FlatAmount;

        //Owner stat adding to the change.
        float owner_add = GetEffectFromStats(owner.Stats, StatsAddingFromOwner);
        float owner_red = GetEffectFromStats(owner.Stats, StatsReducingFromOwner);

        //Apply to each target.
        foreach (Mob target in usage_params.MobsTargeted)
        {
            float target_add = GetEffectFromStats(target.Stats, StatsAddingFromTarget);
            float target_red = GetEffectFromStats(target.Stats, StatsReducingFromTarget);

            float total_target = flat + (target_add + owner_add) - (target_red + owner_red);

            if (InvertResult)
            {
                total_target = -total_target;
            }

            if (ChangeCurrent)
            {
                target.Stats.ChangeValue(StatToChange, total_target);
            }
            if (ChangeMax)
            {
                target.Stats.ChangeMax(StatToChange, total_target);
            }
        }
    }

    protected static float GetEffectFromStats(MobStatSet stats, Dictionary<EStatName, float> stat_dict)
    {
        float output = 0;
        foreach (var item in stat_dict)
        {
            output += stats.GetValue(item.Key) * item.Value;
            output = Math.Max(output, 0);
        }

        return output;
    }

    public EffectStatChange SetOwnerAddingBoost(EStatName stat, float modifier)
    {
        StatsAddingFromOwner[stat] = modifier;
        return this;
    }
    public EffectStatChange SetOwnerReducingBoost(EStatName stat, float modifier)
    {
        StatsReducingFromOwner[stat] = modifier;
        return this;
    }
    public EffectStatChange SetTargetAddingBoost(EStatName stat, float modifier)
    {
        StatsAddingFromTarget[stat] = modifier;
        return this;
    }
    public EffectStatChange SetTargetReducingBoost(EStatName stat, float modifier)
    {
        StatsReducingFromTarget[stat] = modifier;
        return this;
    }
    public EffectStatChange SetFlatAmount(float amount)
    {
        FlatAmount = amount;
        return this;
    }
}



