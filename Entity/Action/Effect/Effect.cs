using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.World;
using static ChessLike.Entity.Action.Ability;
using static ChessLike.Entity.Action.ActionEvent;

namespace ChessLike.Entity.Action;

/// <summary>
/// Decides what effect it will have on targets.
/// By default it can only affect the owner's stats by a flat value, useful for setting a cost.
/// </summary>
public abstract class Effect
{
    public readonly bool CanBePassive = false;

    public void Use(UsageParams usage_params)
    {
        CustomUse(usage_params);
    }

    public virtual string GetDescription()
    {
        return "";
    }

    public abstract void CustomUse(UsageParams usage_params);
}
