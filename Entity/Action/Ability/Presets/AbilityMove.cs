using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityMove : Ability
{
	//TODO: implement variants
	[Export]
	public EMovementMode MovementMode;

    public AbilityMove() : base()
	{
	}

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        Mob owner = usage_params.OwnerRef;
        List<Vector3i> path = usage_params.PositionsTargeted;
        MobCommandMove command = new(path, MovementMode);
        owner.CommandProcess(command);
    }

	public override string GetDescription(bool includeBasics = true, bool assumeIsSetup = true)
	{
		string description = base.GetDescription(includeBasics, assumeIsSetup);
		return description;
    }

    public override string GetUseText(UsageParameters parameters)
    {
        Vector3i position = parameters.PositionsTargeted.First();
        return $"{Owner.DisplayedName} moved to {position}";
    }
}
