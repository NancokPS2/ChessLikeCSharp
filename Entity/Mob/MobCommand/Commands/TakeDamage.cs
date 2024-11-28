using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Command;

public class MobCommandTakeDamage : MobCommand
{
    public float DefenseIgnorePercent = 0;
    public float DefenseIgnoreFlat = 0;
    private float _damage;
    public float Damage {get => _damage; set => _damage = Mathf.Clamp(value, 0, float.MaxValue);}
    public MobCommandTakeDamage(float damage)
    {
        Damage = damage;
    }

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
    }
}
