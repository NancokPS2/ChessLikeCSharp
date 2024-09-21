namespace ChessLike.Entity;

public partial class Action 
{
    public class WalkEffect : Effect
    {
        public override void CustomUse(UsageParams usage_params)
        {
            Mob owner = usage_params.OwnerRef;
            Vector3i target = usage_params.PositionsTargeted[0];
            usage_params.OwnerRef.Position = usage_params.PositionsTargeted[0];
        }
    }

}
