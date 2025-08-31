using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Parameters;
using Godot;

namespace ChessLike.StatusEffect;

[GlobalClass]
public partial class AutoActivationParameters : Resource
{
    [Export]
    public EAutoActivationMode AutoActivationMode;

    [Export]
    public int AutoActivationMax = 3;

    [ExportGroup("Reaction Activation")]
    [Export]
    public bool ActivatedAfterAction = false;

    [Export]
    public bool ActivatedOnlyIfTargetsMe = true;

    [Export]
    public Godot.Collections.Array<EActionFlag> ActivatedByActionWithFlags = new();

    [ExportGroup("Timed Activation")]

    [Export]
    public int ActivatedEveryXTime = 100;

    [ExportGroup("Turn Activation")]
    [Export]
    public bool ActivatedByTurnEnd = false;

    [Export]
    public bool ActivatedOnlyIfTurnIsMine = true;

    public bool IsActionWithValidFlags(ActionEvent action)
    {
        foreach (var flag in ActivatedByActionWithFlags)
        {
            //If none of the flags required are in the action, it isn't valid.
            if (action.Flags.Contains(flag))
                return true;
        }
        return false;
    }
}
