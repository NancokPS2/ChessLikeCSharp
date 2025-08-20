using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using Godot;

namespace ChessLike.Entity.Action.Preset;

[GlobalClass]
public partial class AbilityAttack : Ability
{
	[Export]
	public Godot.Collections.Dictionary<EStatName, float> StatModifiers = new();

	[Export]
	public float DamageBase = 0;

    public AbilityAttack() : base()
	{
	}

    public override void Use(UsageParameters usage_params)
    {
        base.Use(usage_params);
        float damage = GetDamage(usage_params);
        MobCommandTakeDamage command = new MobCommandTakeDamage(damage);
        foreach (var item in usage_params.MobsTargeted)
        {
            item.CommandProcess(command);
        }
    }


	public float GetDamage(UsageParameters usage)
	{
		float total = DamageBase;
		Mob owner = usage.OwnerRef;
		
		//Apply modifiers from stats
		foreach (var item in StatModifiers)
		{
			total += owner.Stats.GetStat(item.Key) * item.Value;
		}

		return total;
	}

	public override string GetUseText(UsageParameters parameters)
    {
        string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
        return $"{Owner.DisplayedName} attacked {targets} for {GetDamage(parameters)} damage";
    }

    public override string GetDescription()
    {
        return $"Attack a target with your weapon, dealing {Owner.Stats.GetStat(EStatName.STRENGTH) / 2} damage. (50% {EStatName.STRENGTH})";
    }
}
