using ChessLike.Entity.Action;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using ChessLike.Shared;
using ChessLike.StatusEffect;
using ChessLike.Storage;
using ChessLike.Turn;
using ChessLike.World;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types.Sources;
using Godot;

namespace ChessLike.Entity;

[GlobalClass]
public partial class Mob : Resource
{
	protected static List<Mob> Instances = new();

	[Export]
	public string DisplayedName = "UNNAMED";

	[Export]
	private Godot.Collections.Array<ActionEvent> actions
	{
		set => Actions = new(value);
		get => new(Actions);
	}
	private List<ActionEvent> Actions = new();

	public List<MobTemplate> Templates = new();
	[Export]
	private Godot.Collections.Array<MobTemplate> templates
	{
		set => Templates = new(value);

		get => new(Templates);
	}

	[Export]
	public EFaction Faction = EFaction.NEUTRAL;

	private EMobState mobState = EMobState.BENCHED;
	public EMobState MobState
	{
		get => mobState;

		set
		{
			mobState = value;
			EventBus.MobStateChanged?.Invoke(this, mobState);
		}

	}

	public MobStatSet Stats = MobStatSet.GetDefault();

	private Vector3i Position;

	public Mob()
	{
		//TODO: Move this somewhere else
		Instances.Add(this);

		//Default stats
		Stats = MobStatSet.GetDefault();
	}

	public void ResetForCombat()
	{
		TemplateUpdate(true, true);
		EquipmentStatBoostsUpdate();
	}

	#region Instances
	public static List<Mob> GetInstancesInCombat()
		=> Instances.FilterInCombat();

	public static List<Mob> GetInstancesInPosition(Vector3i position)
		=> Instances.FilterInPosition(position);

	public static List<Mob> GetInstancesInFaction(EFaction faction)
		=> Instances.FilterInFaction(faction);
	#endregion

	#region MobTemplate
	public List<string> GetRaceNames()
		=> (from template in Templates where template is MobTemplateRace select template.TemplateName).ToList();

	public List<string> GetJobNames()
		=> (from template in Templates where template is MobTemplateRace select template.TemplateName).ToList();

	public void TemplateSet(MobTemplateBase template)
		=> TemplateSet(new List<MobTemplateBase>() { template });

	public void TemplateSet(MobTemplateRace template)
		=> TemplateSet(new List<MobTemplateRace>() { template });

	public void TemplateSet(MobTemplateJob template)
		=> TemplateSet(new List<MobTemplateJob>() { template });

	public void TemplateSet(MobTemplateIdentity template)
		=> TemplateSet(new List<MobTemplateIdentity>() { template });

	protected void TemplateSet<TTemplate>(List<TTemplate> template)
	where TTemplate : MobTemplate
	{
		TemplateClear<TTemplate>();
		Templates.AddRange(template);
		TemplateUpdate(false, true);
	}

	public List<TTemplate> TemplateGet<TTemplate>()
	where TTemplate : MobTemplate
	{
		List<TTemplate> output = new();
		foreach (var item in Templates)
		{
			if (item is TTemplate tTemp) output.Add(tTemp);
		}
		return output;
	}

	protected void TemplateClear()
		=> TemplateClear<MobTemplate>();

	protected void TemplateClear<TTemplate>()
	where TTemplate : MobTemplate
	{
		Templates.RemoveAll(x => x is TTemplate);
		TemplateUpdate(false, true);
	}

	public void TemplateUpdate(bool refillValues, bool reset)
	{
		if (Templates.Count == 0) return;

		//Make sure there is only one base template
		if (TemplateGet<MobTemplateBase>().Count() != 1)
			throw new Exception(
				$"More than one MobTemplateBase found ({TemplateGet<MobTemplateBase>().Count()})"
				);

		if (reset) MobTemplate.Reset(this);

		TemplateGet<MobTemplateBase>().First().ApplyTemplate(this);

		TemplateGet<MobTemplateRace>().ForEach(x => x.ApplyTemplate(this));

		TemplateGet<MobTemplateJob>().ForEach(x => x.ApplyTemplate(this));

		TemplateGet<MobTemplateIdentity>().ForEach(x => x.ApplyTemplate(this));

		if (refillValues) Stats.RefillValues();

		MsgLog.LogInfoMsg($"Updated templates for mob {DisplayedName} (Refilled? {refillValues})\n"
		+ $"Base: {TemplateGet<MobTemplateBase>().ToStringList()}|"
		+ $"Race: {TemplateGet<MobTemplateRace>().ToStringList()}|"
		+ $"Job: {TemplateGet<MobTemplateJob>().ToStringList()}|"
		+ $"Identity: {TemplateGet<MobTemplateIdentity>().ToStringList()}"
		);
	}
	#endregion


	#region Inventory
	[Export]
	public MobEquipmentInventory EquipmentInventory
	{
		get
		{
			return equipmentInventory;
		}

		set
		{
			equipmentInventory.InventoryChanged -= OnInventoryChanged;
			equipmentInventory = value;
			equipmentInventory.InventoryChanged += OnInventoryChanged;
			EquipmentStatBoostsUpdate();

		}
	}

	private MobEquipmentInventory equipmentInventory = new();

	public void EquipmentStatBoostsUpdate()
	{
		MobStatBoost outputStatBoost = new(ItemEquipment.BOOST_SOURCE);

		foreach (Item item in EquipmentInventory.GetItems())
		{
			ItemEquipment equipment;

			//Make sure it is equipment
			if (item is ItemEquipment _equip) equipment = _equip;
			else throw new Exception($"This inventory is for equipment only. Found {item}");

			outputStatBoost = outputStatBoost + equipment.StatBoost;
		}

		Stats.BoostAdd(outputStatBoost, true);
		MsgLog.LogInfoMsg($"{DisplayedName} updated its inventory boost {outputStatBoost}");
	}

	private void OnInventoryChanged(MobEquipmentInventory inventory)
	{
		EquipmentStatBoostsUpdate();
	}
	#endregion

	#region Faction
	public AStar3D Navigation;
	public Faction GetFaction()
		=> Global.ManagerFaction.ResourceGet(
			Global.ManagerFaction.FindIdentifier(Faction) ?? throw new Exception(),
			true,
			true);

	#endregion

	#region Movement
	public Vector3i GetPosition()
	{
		return Position;
	}

	public void SetPosition(Vector3i where)
	{
		Position = where;
	}

	public List<EMovementMode> GetMovementModesFromAbilities()
		=> (from ability in GetAbilities() select ability.TargetParams.TargetingUsesPathing)
			.Concat(from ability in GetAbilities() select ability.TargetParams.AoEUsesPathing)
			.Distinct()
			.Where(x => x != EMovementMode.INVALID)
			.ToList();

	public List<ECellFlag> CellStandWhitelist = new();
	public List<ECellFlag> CellStandBlacklist = new();
	public List<ECellFlag> CellExistWhitelist = new();
	public List<ECellFlag> CellExistBlacklist = new();

	/// <summary>
	/// Reads current abilities to change what cells the Mob is allowed to be in.
	/// If none are allowed, it defaults to EMovementMode.GROUNDED
	/// </summary>
	/// <param name="movementModes">The movement modes used to decide which cells are valid.</param>
	public void UpdateCellList()
	{
		CellStandWhitelist.Clear();
		CellStandBlacklist.Clear();
		CellExistWhitelist.Clear();
		CellExistBlacklist.Clear();

		//By default, can only exist in air, not in solid materials
		CellExistWhitelist.Add(ECellFlag.AIR);
		CellExistBlacklist.Add(ECellFlag.SOLID);
		CellStandWhitelist.Add(ECellFlag.SOLID);

		List<EMovementMode> movementModes = GetMovementModesFromAbilities();

		foreach (var moveMode in movementModes)
		{
			switch (moveMode)
			{
				case EMovementMode.GROUNDED:
					break;

				case EMovementMode.FLY:
					CellStandWhitelist.Add(ECellFlag.AIR);
					break;

				case EMovementMode.AMPHIBIOUS:
					CellExistWhitelist.Add(ECellFlag.LIQUID);
					break;

				default: break;
			}
		}
		EventBus.MobCellListChanged?.Invoke(this);
	}

	public void Move(Vector3i to, MovementParameters moveParams)
		=> Move(new List<Vector3i>() { to }, moveParams);
	public void Move(List<Vector3i> path, MovementParameters moveParams)
	{
		if (path.Count == 0) MsgLog.Log(EMessageType.ERROR, "Received empty path.");

		/* foreach (var position in path)
		{
			switch (moveType)
			{
				case EMovementMode.TELEPORT:
					Vector3i original_pos = Position;
					Position = position;
					break;
			}
		}
 */
		EventBus.MobMovementPathRequested?.Invoke(this, path, moveParams);
	}

	public void MoveRelative(Vector3i relative, MovementParameters moveParams)
	{
		Move(GetPosition() + relative, moveParams);
	}

	#endregion

	#region Actions

	public void AddAction<T>(T action, bool copy = true) where T : ActionEvent
		=> AddAction(new List<T>() { action }, copy);

	public void AddAction<T>(List<T> actions, bool copy = true) where T : ActionEvent
	{
		foreach (var action in actions)
		{
			ActionEvent newAction = copy ? (T)action.Duplicate(true) : action;
			newAction.Setup(this);
			Actions.Add(newAction);
			EventBus.MobActionAdded?.Invoke(this, newAction);
		}

		//Update the cell list if any of the actions has movement.
		if (actions.Any(
			x => x.TargetParams.TargetingUsesPathing != EMovementMode.INVALID || x.TargetParams.AoEUsesPathing != EMovementMode.INVALID)
			)
			UpdateCellList();
	}

	public void RemoveAction(ActionEvent action) => RemoveAction(new List<ActionEvent>() { action });

	public void RemoveAction(List<ActionEvent> actions)
	{
		List<ActionEvent> to_delete = new(actions);
		foreach (var item in to_delete)
		{
			Actions.Remove(item);
			EventBus.MobActionRemoved?.Invoke(this, item);
		}

	}

	public void ClearAction()
		=> new List<ActionEvent>(Actions)
		.ForEach(x => RemoveAction(x));

	public List<Ability> GetAbilities()
	{
		List<Ability> output = new();
		foreach (var item in Actions)
		{
			if (item is Ability abil)
			{
				output.Add(abil);
			}
		}
		return output;
	}
	#endregion

	#region Per Turn Values
	public bool TurnActive;
	#endregion

	#region Status Effects
	protected UniqueList<Status> StatusEffectsApplied = new();
	public void AddStatusEffect(Status status)
	{
		StatusEffectsApplied.Add(status, true);
		status.TargetMob = this;
		status.Setup(this);
		EventBus.StatusEffectAdded?.Invoke(this, status);
	}

	public void RemoveStatusEffect(Status status)
	{
		status.UnSetup();
		StatusEffectsApplied.Remove(status);
		status.TargetMob = null;
		EventBus.StatusEffectRemoved?.Invoke(this, status);
	}

	public void RemoveStatusEffect()
	{
		new List<Status>(StatusEffectsApplied).ForEach(x => RemoveStatusEffect(x));
	}

	public List<Status> GetAllStatusEffects()
		=> new(StatusEffectsApplied);

	#endregion

	#region Misc

	public bool IsInCombat() => mobState == EMobState.COMBAT;

	public override string ToString()
	{
		string output = $"Name: {DisplayedName} \nFaction: {Faction} \nRace: {GetRaceNames().ToStringList()} \n";

		return output;
	}

	public string ToStringStats() => $"---\nStats: {Stats}";

	public string ToStringActions()
	{
		string output = "";
		output += $"---\nAbilities: {GetAbilities().ToStringList()}";
		//output += $"---\nPassives: {GetPassives().ToStringList()}";
		output += $"---\nTemplates: {Templates.ToStringList()}";
		return output;
	}

	public static Mob GetDefault()
	{
		Mob output = new Mob();
		output.DisplayedName = "DEFAULT MOB";
		return output;
	}
	#endregion
}

#region Extension
public static class MobExtensions
{
	public static List<Mob> FilterInCombat(this List<Mob> mobs)
		=> mobs.Where(x => x.MobState == EMobState.COMBAT).ToList();

	public static List<Mob> FilterInPosition(this List<Mob> mobs, Vector3i position)
		=> mobs.Where(x => x.GetPosition() == position).ToList();

	public static List<Mob> FilterInFaction(this List<Mob> mobs, EFaction faction)
		=> mobs.Where(x => x.Faction == faction).ToList();

	public static List<Mob> FilterHostileToFaction(this List<Mob> mobs, EFaction factionToCheck)
	{
		List<Mob> output = new();
		Faction thisFaction = Global.ManagerFaction.ResourceGet(factionToCheck, true) ?? throw new Exception("No faction exists with this enum");

		foreach (var mob in mobs)
		{
			Faction otherFaction = Global.ManagerFaction.ResourceGet(mob.Faction, true);

			if (thisFaction.IsEnemy(otherFaction.Identifier))
			{
				output.Add(mob);
			}
		}

		return output;
	}
}
#endregion