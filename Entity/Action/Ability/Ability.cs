using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using ChessLike.Entity;
using ChessLike.Entity.Command;
using ChessLike.Shared.Identification;
using ChessLike.World;

namespace ChessLike.Entity.Action;

/// <summary>
/// Usage:
/// Start by creating an Effect
/// 
/// Create an UsageParams object to use with the Effect
/// Set UsageParams.owner and grid. 
/// Use IsTargetingValid() to filter a valid result for UsageParams.location_selected 
/// Pass the result of GetLocationsTargeted() to UsageParams.locations_targeted.
/// Pass the result of GetTargetsAffected() to UsageParams.mob_targets
/// 
/// </summary>
public partial class Ability : ActionEvent
{
    public EAbility Identifier = EAbility.NULL;

    public MobFilterParameters FilterParams = new();
    public TargetingParameters TargetParams = new();

    public override void Use(UsageParameters usage_params)
    {
        EventBus.AbilityUsed?.Invoke(usage_params);
    }
}

