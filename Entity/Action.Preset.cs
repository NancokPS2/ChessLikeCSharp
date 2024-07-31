using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    public Action PresetDealDamage(float amount)
    {
        effect_params.StatAddition[StatSet.Name.HEALTH] = amount;
        return this;
    }
}
