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
	public float Damage
	{
		get => damage;
		set
		{
			damage = Mathf.Clamp(value, 0, float.MaxValue);
		}
	}
    private float damage;

    [Export]
    public float DefenseRatioAccounted = 1;
    [Export]
    public float DefenseIgnoreFlat = 0;

    public MobCommandTakeDamage()
    {
    }


	public override void UseCommand(Mob mob)
	{
		base.UseCommand(mob);
		float defense = mob.Stats.GetStat(EStatName.DEFENSE) * DefenseRatioAccounted - DefenseIgnoreFlat;
		float healthLoss = Damage - defense;
		float change = mob.Stats.ChangeValue(EValueName.HEALTH, Math.Min(-healthLoss, 0));

		EventBus.MobStatValueChanged?.Invoke(mob, EValueName.HEALTH, change);

		MsgLog.LogGameMsg($"{mob.DisplayedName} took {change} damage.");
    }
}
