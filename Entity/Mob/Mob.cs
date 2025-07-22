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

    public Dictionary<MobTemplate.ETemplateType, List<MobTemplate>> Templates = new();
    [Export]
    private Godot.Collections.Dictionary<MobTemplate.ETemplateType, Godot.Collections.Array<MobTemplate>> templates
    {
        set
        {
            foreach (var item in value)
            {
                Templates[item.Key] = item.Value.ToList();
            }
        }

        get
        {
            Godot.Collections.Dictionary<MobTemplate.ETemplateType, Godot.Collections.Array<MobTemplate>> output = new();
            foreach (var item in Templates)
            {
                Templates[item.Key] = item.Value;
            }
            return output;
        }
    }

    [Export]
    public ERace Race = ERace.HUMAN;

    [Export]
    public EFaction Faction = EFaction.NEUTRAL;

    public EMobMovementMode MovementMode { set => SetMovementMode(value); get => movementMode; }
    private EMobMovementMode movementMode;

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

        //Make sure the Templates array has Lists
        Templates.AddValueCollections();

        SetupEventBus();
    }
    #region MobTemplate
    protected void TemplateResetToBase()
    {
        //Make sure there is exactly 1 base template.
        if (Templates[MobTemplate.ETemplateType.BASE].Count() != 1)
            throw new Exception($"There is more than one or NO {MobTemplate.ETemplateType.BASE} template.\n{Templates.ToStringList()}");
    }

    protected void TemplateSet(MobTemplate.ETemplateType type, ICollection<MobTemplate> templates)
    {
        if (templates.Any(x => x.Type != type))
            throw new Exception($"Mismatched type of template, was setting {type}.\nFound templates:\n{templates.ToStringList()}");

        Templates[type] = templates.ToList();
    }

    protected void TemplateClear(MobTemplate.ETemplateType type)
    {
        Templates[type].Clear();
    }

    protected void TemplateUpdate()
    {
        TemplateResetToBase();
        foreach (var item in Templates)
        {
            if (item.Key == MobTemplate.ETemplateType.BASE) continue;

            item.Value.ForEach(x => x.ApplyTemplate(this));
        }
    }
    #endregion

    #region Inventory
    [Export]
    public MobEquipmentInventory EquipmentInventory = new();

    protected void UpdateEquipmentStatBoosts()
    {
        MobStatBoost outputStatBoost = new(ItemEquipment.BOOST_SOURCE);

        foreach (Item item in EquipmentInventory.GetItems())
        {
            ItemEquipment equipment;

            //Make sure it is equipment
            if (item is ItemEquipment _equip) equipment = _equip;
            else throw new Exception($"This inventory is for equipment only. Found {item}");

            outputStatBoost = (MobStatBoost)(outputStatBoost + equipment.StatBoost);
        }

        Stats.BoostAdd(outputStatBoost, true);
    }

    #endregion

    #region Faction
    public Faction GetFaction()
        => Global.ManagerFaction.GetPooledByEnum(Faction);

    #endregion

    #region Movement
    public void Move(Vector3i to)
    {
        Vector3i original_pos = Position;
        Position = to;
        EventBus.MobMoved?.Invoke(this, original_pos, Position);
    }

    public void MoveRelative(Vector3i to)
    {
        Move(GetPosition() + to);
    }

    public void MoveTroughPath(List<Vector3i> path)
    {
        foreach (var item in path)
        {
            Move(item);
        }
        EventBus.MobFinishedPathMove?.Invoke(this, path);
    }

    #endregion

    #region Actions
    private Ability _movement = new();

    public void SetMovementMode(EMobMovementMode mode)
    {
        Actions.Remove(_movement);
        _movement = new AbilityMove(EMobMovementMode.WALK);
        AddAction(_movement);
        movementMode = mode;
    }

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
    protected bool TurnActive;
    #endregion

    #region Misc
    
    public bool IsInCombat() => mobState == EMobState.COMBAT;

	public override string ToString()
    {
        string output = $"Name: {DisplayedName} \nFaction: {Faction} \nRace: {Race} \n";

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
    #endregion
}