using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using Godot;

namespace ChessLike.Entity.Action.Preset;

[GlobalClass]
public partial class AbilityMove : Ability
{
    //TODO: implement variants

    public AbilityMove() : base()
    {
    }

    public AbilityMove(EMobMovementMode variant) : base()
    {
    }

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        Mob owner = usage_params.OwnerRef;
        Vector3i target = usage_params.PositionsTargeted[0];
        MobCommandTeleport command = new(target);
        owner.CommandProcess(command);
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
