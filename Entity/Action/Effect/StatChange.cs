namespace ChessLike.Entity;

public partial class Action 
{
public partial class Effect
    {
        public class StatChange : Effect
        {
            //Which stat of the owner to change.
            public StatName? owner_stat_to_change = null;

            //Which stat of the target to change.
            public StatName? target_stat_to_change = null;

            //Sum this stat from the owner to the value.
            public StatName? owner_stat_sum = null;

            //Sum this stat from the target to the value.
            public StatName? target_stat_sum = null;

            //How much to change it as a base.
            public float flat_amount = 0;

            //Apply a final modifier to the value.
            public float final_modifier = 1.0f;

            public static StatChange PresetChangeTargetBasedOnOwnerStat(StatName stat_from_owner_to_sum, StatName stat_to_affect)
            {
                StatChange output = new();
                output.owner_stat_sum = stat_from_owner_to_sum;
                output.target_stat_to_change = stat_to_affect;
                return output;
            }

            public override void CustomUse(UsageParams usage_params)
            {
                throw new NotImplementedException();
            }
        }

    }
}
