namespace ChessLike.Entity;

public partial class Action 
{
    public class AttackEffect : Effect
    {
        public StatName based_on_stat = StatName.STRENGTH;
        public float stat_modifier = 0.5f;

        public override void CustomUse(UsageParams usage_params)
        {
            float damage = usage_params.OwnerRef.Stats.GetValue(based_on_stat) * stat_modifier;

            foreach (Mob target in usage_params.MobsTargeted)
            {
                target.Stats.ChangeValue(StatName.HEALTH, damage, modifiers);
            }
        }

        public AttackEffect(StatName based_on_stat = StatName.STRENGTH, float stat_modifer = 0.5f)
        {
            this.based_on_stat = based_on_stat;
            this.stat_modifier = stat_modifer;
        }
    }

}

