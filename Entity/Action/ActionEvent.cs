using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public abstract partial class ActionEvent
{
    public Mob Owner;

    public string Name = "Undefined Action";

    public AnimationParameters AnimationParams = new();
    public MobFilterParameters MobFilterParams = new();

    public List<EActionFlag> Flags = new();

    public abstract void Use(UsageParameters usage_params);
}
