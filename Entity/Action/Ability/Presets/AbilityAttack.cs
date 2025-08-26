using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using ExtendedXmlSerializer;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityAttack : Ability
{

	[Export]
	public Godot.Collections.Dictionary<EStatName, float> StatModifiers = new();

	[Export]
	public Godot.Collections.Dictionary<EValueName, float> ValueModifiers = new();

	[Export]
	public float DamageBase = 0;

	[Export]
	public float DamageMinimum = 0;

	[Export(PropertyHint.Range, "0, 1, 0.05")]
	public float DefensePiercing = 0;

	[Export(PropertyHint.Range, "0,1,0.01")]
	public float HealthLossPercent = 0;

	public AbilityAttack() : base()
	{
		Description = "Attack a target with your weapon, dealing damage based on your {StatModifiers}. For around {Damage} damage";
	}

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);
		float damage = GetDamage();
		MobCommandTakeDamage command = new MobCommandTakeDamage()
		{
			Damage = damage,
			DefenseRatioAccounted = 1 - DefensePiercing,
		};
		foreach (var item in usageParams.MobsTargeted)
		{
			item.CommandProcess(command);
		}

		if (HealthLossPercent > 0)
		{
			float ownerHealthLoss = usageParams.OwnerRef.Stats.GetStat(EStatName.HEALTH) * HealthLossPercent;
			MobCommandTakeDamage selfDamage = new()
			{
				Damage = ownerHealthLoss,
				DefenseRatioAccounted = 0
			};
			usageParams.OwnerRef.CommandProcess(selfDamage);
		}
	}


	public virtual float GetDamage()
	{
		return GetDamageOwner(Owner);
	}

	private float GetDamageOwner(Mob owner)
	{
		float total = DamageBase;

		//Apply modifiers from stats
		foreach (var item in StatModifiers)
		{
			total += owner.Stats.GetStat(item.Key) * item.Value;
		}

		//Apply modifiers from stat values
		foreach (var item in ValueModifiers)
		{
			total += owner.Stats.GetValue(item.Key) * item.Value;
		}

		return Mathf.Max(total, DamageMinimum);
	}


	public override string GetUseText(UsageParameters parameters)
	{
		string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
		return $"{Owner.DisplayedName} attacked {targets} for {GetDamage()} damage";
	}

	public override string GetDescription(bool includeBasics = true)
	{
		string output = base.GetDescription();
		output = output.Format(
			new Dictionary<string, string>()
			{
				{"StatModifiers", StatModifiers.ToStringList()},
				{"ValueModifiers", ValueModifiers.ToStringList()},
				{"DamageMinimum", DamageMinimum.ToString()},
				{"DamageBase", DamageBase.ToString()},
				{"Damage", GetDamage().ToString()},
				{"HealthLossPercent", HealthLossPercent.ToString()}
			}
		);
		return output;
	}

}
