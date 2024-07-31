using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity;

public partial class Action
{
    public Action PresetDealDamage(float amount)
    {
        effect_params.TargetStatChangeValue[StatSet.Name.HEALTH] = amount;
        return this;
    }

    public Action PresetPushBack(int distance)
    {
        effect_params.PositionChange = Vector3i.FORWARD * distance;
        return this;
    }

    public Action PresetConsumeEnergy(float amount)
    {
        effect_params.OwnerStatChangeValue[StatSet.Name.ENERGY] = amount;
        return this;
    }
}
