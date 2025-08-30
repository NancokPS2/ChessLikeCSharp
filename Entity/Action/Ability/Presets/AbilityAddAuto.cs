using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace ChessLike.Entity.Action;

[GlobalClass]
public partial class AbilityAddAuto : Ability
{
	const string META_IMPRINTED = "AbilityToggleAutoIMPRINTED_IDENTIFIER";

	public Ability AddedAbility = null!;
	[Export]
	private Ability addedAbility
	{
		get => AddedAbility;
		set
		{
			AddedAbility = value;
			if (!Validate(AddedAbility))
				throw new Exception("Test exception to make sure this is set correctly.");
		}
	}

	[Export]
	public int MaxStacks = 1;

	protected List<Ability> StacksTracker = new();

	public AbilityAddAuto() : base()
	{

	}

	public override void Use(UsageParameters usageParams)
	{
		base.Use(usageParams);

		foreach (var mob in usageParams.MobsTargeted)
		{
			Ability copy = (Ability)AddedAbility.Duplicate(true);
			Imprint(copy);

			//Check for max stacks first.
			if (GetAllStacks().Where(mob.GetAbilities().Contains).Count() >= MaxStacks)
				continue;

			mob.AddAction(
				copy
				);
			StacksTracker.Add(copy);
		}
	}

	public List<Ability> GetAllStacks()
	{
		StacksTracker.RemoveAll(x => !IsInstanceValid(x));

		return StacksTracker;
	}

	protected void Imprint(Ability ability)
	{
		if (ability != AddedAbility) throw new Exception();
		ability.SetMeta(GetImprintMetaKey(), true);
	}

	protected bool IsImprinted(Ability ability)
		=> ability.GetMeta(GetImprintMetaKey(), false).As<bool>();

	protected string GetImprintMetaKey()
		=> META_IMPRINTED + Owner.GetInstanceId().ToString() + GetInstanceId();

	protected bool Validate(Ability ability)
	{
		bool isAutoActivated = ability.AutoActivationParams.AutoActivationMode != Parameters.EAutoActivationMode.NONE;
		bool isFree = ability.CostParams.IsFree();
		return isAutoActivated && isFree;
	}
}
		