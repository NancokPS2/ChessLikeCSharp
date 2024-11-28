using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessLike.Entity.Action;

public partial class ActionEvent
{
    public Mob Owner;

    public string Name = "Undefined Action";

    public AnimationParameters AnimationParams = new();
    public MobFilterParameters MobFilterParams = new();

    public List<EFlag> Flags = new();

    public List<Effect> Effects = new();


    public virtual void Use(UsageParameters usage_params)
    {
        foreach (Effect effect in Effects)
        {
            effect.Use(usage_params);
        }
    }
}
