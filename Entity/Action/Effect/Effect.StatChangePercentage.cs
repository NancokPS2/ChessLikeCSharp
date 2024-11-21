namespace ChessLike.Entity.Action;

public class EffectStatChangePercentageTODO : EffectStatChange
{
    //TODO
    public override void CustomUse(Ability.UsageParameters usage_params)
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

}



