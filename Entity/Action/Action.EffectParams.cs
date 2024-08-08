using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;

namespace ChessLike.Entity;

public partial class Action 
{
    /// <summary>
    /// Decides what effect it will have on targets.
    /// By default it can only affect the owner's stats by a flat value, useful for setting a cost.
    /// </summary>
    public partial class EffectParams
    {
        public float energy_cost = 0;
        public float health_cost = 0;
        public FloatRes.Modifier[] modifiers = Array.Empty<FloatRes.Modifier>();
        public void Use(UsageParams usage_params)
        {

            usage_params.owner.Stats.ChangeValue(StatSet.Name.HEALTH, health_cost);
            usage_params.owner.Stats.ChangeValue(StatSet.Name.ENERGY, energy_cost);

            CustomUse(usage_params);
        }

        public virtual void CustomUse(UsageParams usage_params)
        {
            
        }


        public class StatChange : EffectParams
        {
            //Which stat of the owner to change.
            public StatSet.Name? owner_stat_to_change = null;

            //Which stat of the target to change.
            public StatSet.Name? target_stat_to_change = null;

            //Sum this stat from the owner to the value.
            public StatSet.Name? owner_stat_sum = null;

            //Sum this stat from the target to the value.
            public StatSet.Name? target_stat_sum = null;

            //How much to change it as a base.
            public float flat_amount = 0;

            //Apply a final modifier to the value.
            public float final_modifier = 1.0f;

            public static StatChange PresetChangeTargetBasedOnOwnerStat(StatSet.Name stat_from_owner_to_sum, StatSet.Name stat_to_affect)
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

        public class RecoverPercentOfMax : EffectParams
        {
            public StatSet.Name stat_to_recover = StatSet.Name.HEALTH;
            public float percentage = 0.25f;

            public override void CustomUse(UsageParams usage_params)
            {
                foreach (Mob target in usage_params.mob_targets)
                {
                    float to_recover = target.Stats.GetMax(stat_to_recover) * percentage;
                    target.Stats.ChangeValue(stat_to_recover, to_recover, modifiers);
                }
            }
        }

        public class Attack : EffectParams
        {
            public StatSet.Name based_on_stat = StatSet.Name.STRENGTH;
            public float stat_modifier = 0.5f;

            public override void CustomUse(UsageParams usage_params)
            {
                float damage = usage_params.owner.Stats.GetValue(based_on_stat) * stat_modifier;

                foreach (Mob target in usage_params.mob_targets)
                {
                    target.Stats.ChangeValue(StatSet.Name.HEALTH, damage, modifiers);
                }
            }

            public Attack(StatSet.Name based_on_stat = StatSet.Name.STRENGTH, float stat_modifer = 0.5f)
            {
                this.based_on_stat = based_on_stat;
                this.stat_modifier = stat_modifer;
            }
        }

        public class Walk : EffectParams
        {
            public override void CustomUse(UsageParams usage_params)
            {
                IGridPosition owner = usage_params.owner;
                Vector3i target = usage_params.locations_targeted[0];
                usage_params.owner.GridPosition = usage_params.locations_targeted[0];
            }
        }

    }
}
