using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Extension;

namespace ChessLike.Entity.Action;

public abstract partial class ActionEvent
{
    private Mob? owner;
    public Mob Owner { get => owner ?? throw new Exception("Owner should be set before usage."); set => owner = value; }

    public string Name = "Undefined Action";

    public AnimationParameters AnimationParams = new();
    public MobFilterParameters MobFilterParams = new();

    public List<EActionFlag> Flags = new();



    public abstract void Use(UsageParameters usage_params);
    public abstract void OnAddedToMob();
    public abstract void OnRemovedFromMob();
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
