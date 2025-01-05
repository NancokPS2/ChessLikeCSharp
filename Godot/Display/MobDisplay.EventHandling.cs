using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;

namespace Godot.Display;

public partial class MobMeshDisplay
{

    public MobMeshDisplay()
    {
        EventBus.MobTurnStarted += OnTurnStarted;
        EventBus.MobStatChanged += OnMobStatValueChanged;
    }

    public void OnMobStatValueChanged(Mob mob, StatName stat, float amount)
    {
        MobDisplayComponent comp = GetComponent(mob);
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

        //TODO: I don't think this does anything at this point.
        GetTree().Root.AddChild(
            new PopText(){
                Text = amount.ToString(), 
                Direction = Vector3.Up, 
                TextColor = color,
                GlobalPosition = global_pos,
                AnimationMode = PopText.Animation.SHAKE_AT_END
            }
        );
    }

    public void OnTurnStarted(Mob who)
    {
        MobDisplayComponent component = MobComponents[who];
        Vector3 global_pos = component.GetPositionGlobal();
        GetTree().Root.AddChild( 
            new PopText(){
                Text = "READY",
                GlobalPosition = global_pos,
                AnimationMode = PopText.Animation.SHAKE_AT_END
            }
        );
            
    }
}
