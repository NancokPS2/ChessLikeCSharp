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
    public partial class Effect
    {
        public float energy_cost = 0;
        public float health_cost = 0;
        public ClampFloat.Modifier[] modifiers = Array.Empty<ClampFloat.Modifier>();
        public void Use(UsageParams usage_params)
        {

            usage_params.OwnerRef.Stats.ChangeValue(StatName.HEALTH, health_cost);
            usage_params.OwnerRef.Stats.ChangeValue(StatName.ENERGY, energy_cost);

            CustomUse(usage_params);
        }

        public virtual void CustomUse(UsageParams usage_params)
        {
            
        }

    }
}
