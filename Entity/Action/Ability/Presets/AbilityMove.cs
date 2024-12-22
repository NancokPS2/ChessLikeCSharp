using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action.Preset;

public class AbilityMove : Ability
{ 
    //TODO: implement variants

    public AbilityMove(EMovementMode variant) : base()
    {
        ChainName("Move");
        ChainIdentifier(EAbility.MOVE);

        TargetParams = new TargetingParameters(){
            TargetingRange = 0,
            TargetingMaxPositions = 1,
            TargetingUsesPathing = true,
            TargetingRangeStatBonus = StatName.MOVEMENT,
        };

        FilterParams = new MobFilterParameters()
        {
            PickMobInTargetPos = false
        };
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        Mob owner = usage_params.OwnerRef;
        Vector3i target = usage_params.PositionsTargeted[0];
        owner.Position = target;
    }

    public override string GetDescription()
    {
        return "Teleport to the target location.";
    }

    public override string GetUseText(UsageParameters parameters)
    {
        Vector3i position = parameters.PositionsTargeted.First();
        return $"{Owner.DisplayedName} moved to {position}";

    }
}
