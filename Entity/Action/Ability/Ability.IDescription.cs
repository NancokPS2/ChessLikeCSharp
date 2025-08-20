using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using ChessLike.Shared.Serialization;
using Godot;

namespace ChessLike.Entity.Action;

public partial class Ability : IDescription
{
    public string GetDescription(bool includeBasics = true)
    {
		string output = "";
		if (includeBasics)
		{
			output.NewLine($"Range: {TargetParams.Range}");

			//If the mode is in SINGLE, do not add anything.
			TargetingParameters.AoEMode aoeMode = TargetParams.AoEShape;
			output.NewLine(aoeMode != TargetingParameters.AoEMode.SINGLE ? $"AoE: {aoeMode}" : "");

			//List costs
			output.NewLine($"Costs: \n{CostParams}");

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
			output.NewLine(canHit);
		}
		output += Description.Format(
			new Dictionary<string, string>()
			{
				{"AbilityName", Name},
				{"OwnerName", Owner.DisplayedName},
			}
		);


		return output;
        return String.Format("Identifier: {0} \nName: {1} \nFilter Parameters: \n{2} \nTarget Parameters: \n{3} \nFlags: {4}",
			Enum.GetName(Identifier),
			Name,
			MobFilterParams.ToString().Indent(@"    "),
			TargetParams.ToString().Indent(@"    "),
			Flags.ToStringList()
		);
    }

    public string GetDescriptiveName() => Name;
}
