namespace ChessLike.Entity;

public partial class Action 
{
    public class RecoverPercentOfMaxEffect : Effect
    {
        public StatName stat_to_recover = StatName.HEALTH;
        public float percentage = 0.25f;

        public override void CustomUse(UsageParams usage_params)
        {
            foreach (Mob target in usage_params.MobsTargeted)
            {
                float to_recover = target.Stats.GetMax(stat_to_recover) * percentage;
                target.Stats.ChangeValue(stat_to_recover, to_recover, modifiers);
            }
        }
    }

}
