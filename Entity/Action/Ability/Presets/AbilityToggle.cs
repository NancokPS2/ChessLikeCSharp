using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

public partial class AbilityToggle : Ability
{
	public Ability ToggledAbility = null!;
	[Export]
	private Ability toggledAbility
	{
		get => ToggledAbility;
		set
		{
			ToggledAbility = (Ability)value.Duplicate(true);
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
			Owner.RemoveAction(ToggledAbility);
		else
			Owner.AddAction(ToggledAbility);
	}

	protected bool IsEnabled()
		=> Owner.GetAbilities().Contains(ToggledAbility);


	protected bool Validate(Ability ability)
	{
		bool isAutoActivated = ability.AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.NONE;
		bool isFree = ability.CostParams.IsFree();
		bool autoActiveEnabled = ToggledAbility.AutoActivationEnabled;
		return isAutoActivated && isFree && autoActiveEnabled;
	}

	public override string GetDescription(bool includeBasics = true)
	{
		return base
			.GetDescription(includeBasics)
			.Format(
				new()
				{
					{"enableOrDisable", IsEnabled() ? "disable" : "enable"}
				}
			);
	}

	public override string GetUseText(UsageParameters parameters)
	{
		return $"{Owner.DisplayedName} {(IsEnabled() ? "enabled" : "disabled")} {Name}";
	}
}
		