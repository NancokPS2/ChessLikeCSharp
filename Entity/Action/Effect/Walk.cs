namespace ChessLike.Entity;

public partial class Action 
{
public partial class Effect
    {
        public class Walk : Effect
        {
            public override void CustomUse(UsageParams usage_params)
            {
                IGridPosition owner = usage_params.owner;
                Vector3i target = usage_params.locations_targeted[0];
                usage_params.owner.Position = usage_params.locations_targeted[0];
            }
        }

    }
}
