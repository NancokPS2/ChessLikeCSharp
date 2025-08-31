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
		float total = DamageBase;

		if (Owner is not null)
			total = GetDamageOwner(Owner, total);

		return Mathf.Max(total, DamageMinimum);
	}

	private float GetDamageOwner(Mob owner, float start)
	{
		//Apply modifiers from stats
		foreach (var item in StatModifiers)
		{
			start += owner.Stats.GetStat(item.Key) * item.Value;
		}

		//Apply modifiers from stat values
		foreach (var item in ValueModifiers)
		{
			start += owner.Stats.GetValue(item.Key) * item.Value;
		}

		return start;
		
	}


	public override string GetUseText(UsageParameters parameters)
	{
		string targets = (from mob in parameters.MobsTargeted select mob.DisplayedName).ToStringList(", ");
		return $"{Owner.DisplayedName} attacked {targets} for {GetDamage()} damage";
	}

	public override string GetDescription(bool includeBasics = true, bool assumeIsSetup = true)
	{
		string output = base.GetDescription(includeBasics, assumeIsSetup);
		Dictionary<string, string> formatDict = assumeIsSetup ?
			new Dictionary<string, string>()
			{
				{"StatModifiers", StatModifiers.ToStringList()},
				{"ValueModifiers", ValueModifiers.ToStringList()},
				{"DamageMinimum", DamageMinimum.ToString()},
				{"DamageBase", DamageBase.ToString()},
				{"Damage", GetDamage().ToString()},
				{"HealthLossPercent", HealthLossPercent.ToString()}
			} :
			new Dictionary<string, string>()
			{
				{"StatModifiers", StatModifiers.ToStringList()},
				{"ValueModifiers", ValueModifiers.ToStringList()},
				{"DamageMinimum", DamageMinimum.ToString()},
				{"DamageBase", DamageBase.ToString()},
				{"Damage", GetDamage().ToString()},
				{"HealthLossPercent", HealthLossPercent.ToString()}
			};
		output = output.Format(formatDict);
		return output;
	}

}
