using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.Action;

public abstract partial class ActionEvent : Resource
{
    private Mob? owner;
    public Mob Owner { get => owner ?? throw new Exception("Owner should be set before usage."); set => owner = value; }

    [Export]
    public string Name = "Undefined Action";

    [Export]
    public AnimationParameters AnimationParams = new();

    [Export]
    public MobFilterParameters MobFilterParams = new();

    [Export]
    public Godot.Collections.Array<EActionFlag> Flags = new();

    public abstract void Use(UsageParameters usage_params);
    public virtual string GetDescription()
    {
        return "Undefined action description.";
    }
    public virtual string GetUseText(UsageParameters parameters)
    {
        return $"{Owner.DisplayedName ?? "ERROR"} did something mysterious to {parameters.MobsTargeted.ToStringList(", ")}";
    }

    public override string ToString() => Name;

}
