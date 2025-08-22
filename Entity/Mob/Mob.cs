using ChessLike.Entity.Action;
using ChessLike.Entity.Action.Preset;
using ChessLike.Entity.MobCommand;
using ChessLike.Extension;
using ChessLike.Shared;
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
        Global.ManagerMob.PooledAdd(this);

        //Default stats
        Stats = MobStatSet.GetDefault();
    }
	#region MobTemplate
	public List<string> GetRaceNames()
		=> (from template in Templates where template is MobTemplateRace select template.TemplateName).ToList();

	public List<string> GetJobNames()
		=> (from template in Templates where template is MobTemplateRace select template.TemplateName).ToList();

    public void TemplateSet(MobTemplateBase template)
        => TemplateSet(new List<MobTemplateBase>(){template});

    public void TemplateSet(MobTemplateRace template)
        => TemplateSet(new List<MobTemplateRace>(){template});

    public void TemplateSet(MobTemplateJob template)
        => TemplateSet(new List<MobTemplateJob>(){template});

    public void TemplateSet(MobTemplateIdentity template)
        => TemplateSet(new List<MobTemplateIdentity>(){template});

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

	public void TemplateUpdate(bool refillValues, bool reset = true)
	{
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
	public List<EMobMovementMode> MovementModes
	{
		set
		{
			movementModes = new(value);
			UpdateCellList();
		}
		get => new(movementModes);
	}
	protected List<EMobMovementMode> movementModes;
	public List<ECellFlag> CellStandWhitelist = new();
	public List<ECellFlag> CellStandBlacklist = new();
	public List<ECellFlag> CellExistWhitelist = new();
	public List<ECellFlag> CellExistBlacklist = new();

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

		foreach(var moveMode in MovementModes)
		{
			switch(moveMode)
			{
				case EMobMovementMode.GROUNDED:
					break;
				
				case EMobMovementMode.FLY:
					CellStandWhitelist.Add(ECellFlag.AIR);
					break;

				case EMobMovementMode.AMPHIBIOUS:
					CellExistWhitelist.Add(ECellFlag.LIQUID);
					break;
				
				default: break;
			}
		}
	}

	public void Move(Grid grid, Vector3i to, EMobMovementMode mode)
		=> Move(MoveGetPath(grid, GetPosition(), to), mode);

	[Obsolete("WIP")]
	public List<Vector3i> MoveGetPath(Grid grid, Vector3i from, Vector3i to)
	{
		List<Vector3i> output = new();

		return output;
	}
	
    public void Move(List<Vector3i> path, EMobMovementMode moveType)
	{
		if (path.Count == 0) MsgLog.Log(EMessageType.ERROR, "Received empty path.");
		foreach (var position in path)
		{
			switch (moveType)
			{
				case EMobMovementMode.TELEPORT:
					Vector3i original_pos = Position;
					Position = position;
					break;
			}
		}

		//EventBus.MobMoved?.Invoke(this, original_pos, Position);
		EventBus.MobFinishedPathMove?.Invoke(this, moveType, path);
	}

    public void MoveRelative(Vector3i to, EMobMovementMode mode)
    {
        Move(new(){GetPosition() + to}, mode);
    }

    #endregion

    #region Actions

    public void AddAction(ActionEvent action) => AddAction(new List<ActionEvent>() { action });

    public void AddAction(List<ActionEvent> actions)
    {
		foreach (var action in actions)
		{
			ActionEvent newAction = (ActionEvent)action.Duplicate(true);
			Actions.Add(newAction);
			newAction.Owner = this;
			EventBus.MobActionAdded?.Invoke(this, newAction);
        }
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
		=> Actions.ForEach(x => RemoveAction(x));

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

    public List<Ability> GetPassives()
        => GetAbilities().Where(x => x.IsPassive()).ToList();
    #endregion

    #region Per Turn Values
    public bool TurnActive;
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