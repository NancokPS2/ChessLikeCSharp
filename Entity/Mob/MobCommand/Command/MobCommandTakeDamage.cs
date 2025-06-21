using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.MobCommand;

[GlobalClass]
public partial class MobCommandTakeDamage : Command
{
    [Export]
    public float DefenseRatioAccounted = 1;
    [Export]
    public float DefenseIgnoreFlat = 0;
    private float _damage;
    [Export]
    public float Damage { get => _damage; set => _damage = Mathf.Clamp(value, 0, float.MaxValue); }
    public MobCommandTakeDamage(float damage)
    {
        Damage = damage;
    }

    public override void UseCommand(Mob mob)
    {
        base.UseCommand(mob);
        float defense = mob.Stats.GetValue(EStatName.DEFENSE) * DefenseRatioAccounted - DefenseIgnoreFlat;
        float health_loss = Damage - defense;
        float change = mob.Stats.ChangeValue(EStatName.HEALTH, Math.Min(-health_loss, 0));

        EventBus.MobStatChanged?.Invoke(mob, EStatName.HEALTH, change);
    }
}
