using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityToggle : Ability
{
	const string META_IMPRINTED = "AbilityToggleAutoIMPRINTED_IDENTIFIER";

	public Ability ToggledAbility = null!;
	[Export]
	private Ability toggledAbility
	{
		get => ToggledAbility;
		set
		{
			ToggledAbility = (Ability)value.Duplicate(true);
			Imprint(ToggledAbility, true);
			if (!Validate(ToggledAbility))
				throw new Exception("Test exception to make sure this is set correctly.");
		}
	}

	public AbilityToggle() : base()
	{

	}

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);

		if (!Validate(ToggledAbility))
			throw new Exception("Could not validate the ability before using it.");

		if (MobFilterParams.CannotAffectOwner)
			throw new Exception($"AbilityToggle {Name} cannot affect its owner, but it is only meant for this purpose.");

		if (IsEnabled())
		{
			Owner?.RemoveAction(ToggledAbility);
		}
		else
		{
			Owner?.AddAction(ToggledAbility, false);
		}
	}

	protected bool IsEnabled()
	{
		if (Owner is null)
			throw new Exception();
		int count = Owner.GetAbilities().Count(x => IsImprinted(x));

		if (count > 1)
			throw new Exception("There should only be one toggled ability max.");
		else
			return count == 1;
	}


	protected void Imprint(Ability ability, bool imprint)
	{
		if (ability != ToggledAbility) throw new Exception();
		ability.SetMeta(GetImprintMetaKey(), imprint);
	}

	protected bool IsImprinted(Ability ability)
		=> ability.GetMeta(GetImprintMetaKey(), false).As<bool>();

	protected string GetImprintMetaKey()
		=> META_IMPRINTED + GetInstanceId().ToString();

	[Obsolete]
	protected bool Validate(Ability ability)
	{
		//bool isAutoActivated = ability.AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.NONE;
		//bool isFree = ability.CostParams.IsFree();
		//bool autoActiveEnabled = ToggledAbility.AutoActivationEnabled;
		//return isAutoActivated && isFree && autoActiveEnabled;
		return false;
	}

	public override string GetDescription(bool includeBasics = true)
	{
		return base
			.GetDescription(includeBasics)
			.Format(
				new()
				{
					{"enableOrDisable", IsEnabled() ? "disable" : "enable"},
					{"toggledAbilityName", ToggledAbility is not null ? ToggledAbility.Name : "ERROR"},
				}
			);
	}

	public override string GetUseText(UsageParameters parameters)
	{
		return $"{Owner.DisplayedName} {(IsEnabled() ? "enabled" : "disabled")} {Name}";
	}
}
		