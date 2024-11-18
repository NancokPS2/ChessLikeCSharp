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


    public List<Effect> Effects = new();


    public virtual void Use(UsageParams usage_params)
    {
        foreach (Effect effect in Effects)
        {
            effect.Use(usage_params);
        }
    }
}
