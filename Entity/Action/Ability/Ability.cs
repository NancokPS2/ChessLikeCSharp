using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using ChessLike.Entity;
using ChessLike.Entity.MobCommand;
using ChessLike.Shared.Identification;
using ChessLike.World;
using Godot;

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

[GlobalClass]
public partial class Ability : ActionEvent
{

	[Export]
	public EAbility Identifier = EAbility.NULL;

	[Export(PropertyHint.MultilineText)]
	public string Description = "Mysterious action!";

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);
	}

	public virtual string GetDescription(bool includeBasics = true)
    {
		string output = "";
		if (includeBasics)
		{
			output = output.NewLine($"Range: {TargetParams.Range}");

			//If the mode is in SINGLE, do not add anything.
			TargetingParameters.AoEMode aoeMode = TargetParams.AoEShape;
			output = output.NewLine(aoeMode != TargetingParameters.AoEMode.SINGLE ? $"AoE: {aoeMode}" : "");

			//List costs
			output = output.NewLine($"Costs: \n{CostParams}");

			//List mob filters
			string canHit;
			if (MobFilterParams.PickMobInTargetPos)
			{
				canHit = "Can hit: "
					+ (MobFilterParams.CannotAffectAlly ? "" : "Allies. ")
					+ (MobFilterParams.CannotAffectEnemy ? "" : "Enemies.");
			} else
			{
				canHit = "";
			}
			output = output.NewLine(canHit);
		}
		//It is not using the parent's GetDescription() method.
		output += Description.Format(
			new Dictionary<string, string>()
			{
				{"AbilityName", Name},
				{"OwnerName", Owner.DisplayedName},
			}
		);


		return output;
    }

    public string GetDescriptiveName() => Name;

}

