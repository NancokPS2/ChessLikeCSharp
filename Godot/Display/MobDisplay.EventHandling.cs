using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;

namespace Godot.Display;

public partial class MobMeshDisplay
{
    public void SetupEventsForMob(Mob mob)
    {
        mob.Stats.StatValueChanged += (StatName stat, float amount) => 
            OnMobStatValueChanged(
                MobComponents[mob], stat, amount
            );
       
    }
    public void ConnectToManager(TurnManager manager)
    {
        manager.TurnStarted += OnTurnStarted;
    }

    public void OnMobStatValueChanged(MobDisplayComponent comp, StatName stat, float amount)
    {
        Vector3 global_pos = comp.GetPositionGlobal();
        Color color = new(1,1,1); 
        int sign = MathF.Sign(amount);
        if (sign == -1)
        {
            color = new(1,0,0);
        }
        else if (sign == 1)
        {
            color = new(0,1,0);
        }

        GetTree().Root.AddChild(new PopText(amount.ToString(), Vector3.Up, color).SetAnimation(PopText.Animation.SHAKE_AT_END));
    }

    public void OnTurnStarted(ITurn who)
    {
        if (who is Mob mob)
        {
            MobDisplayComponent component = MobComponents[mob];
            Vector3 global_pos = component.GetPositionGlobal();
            GetTree().Root.AddChild( new PopText("READY!").SetAnimation(PopText.Animation.SHAKE_AT_END));
            
        }
    }
}
