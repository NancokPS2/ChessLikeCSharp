namespace ChessLike.Entity.Action;

public class EffectTeleport : Effect
{
    public override void CustomUse(Ability.UsageParams usage_params)
    {
        Mob owner = usage_params.OwnerRef;
        Vector3i target = usage_params.PositionsTargeted[0];
        usage_params.OwnerRef.Position = usage_params.PositionsTargeted[0];
    }

}

