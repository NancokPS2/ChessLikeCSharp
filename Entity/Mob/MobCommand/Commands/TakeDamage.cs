using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Command;

public class MobCommandTakeDamage : MobCommand
{
    public float DefenseRatioAccounted = 1;
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
        float defense = mob.Stats.GetValue(StatName.DEFENSE) * DefenseRatioAccounted - DefenseIgnoreFlat;
        float health_loss = Damage - defense;
        mob.Stats.ChangeValue(StatName.HEALTH, Math.Min(-health_loss, 0));
    }
}
