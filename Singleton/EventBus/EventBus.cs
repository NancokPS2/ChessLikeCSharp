using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Command;
using ChessLike.Shared.Storage;
using Godot;
using Godot.WorldMap;
using static ChessLike.Entity.Action.ActionEvent;

/// <summary>
/// This should be used ONLY for visual effects and other things detached from the direct logic loop.
/// Hosts nearly all delegates.
/// </summary>
public partial class EventBus : Node
{
    public static EventBus Instance;
    public delegate void Event();
    public delegate void ObjectChanged<T>(T obj);
    public delegate void StringEvent(string text);

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    //World map
    public delegate void MapMarkerEvent(MapMarker3D marker);
    public static MapMarkerEvent? MarkerSelected;

    //Save profile
    public static StringEvent? ProfileNameChanged;

    //Encounter
    public static Event? RoundEnded;

    #region Mobs
    public delegate void MobEvent(Mob mob);
    public static MobEvent? MobTurnStarted;
    public static MobEvent? MobTurnEnded;

    //Mob stats
    public delegate void MobEventStat(Mob mob, StatName stat);
    public static MobEventStat? MobStatChanged;

    //Movement
    public delegate void MobMovement(Mob mob, Vector3i from, Vector3i to);
    public delegate void MobMovementPath(Mob mob, List<Vector3i> path);
    public static MobMovement? MobMoved;
    public static MobMovementPath? MobFinishedMoving;

    //Mob commands
    public delegate void MobCommandEvent(Dictionary<EInfo, string> dict);
    public static MobCommandEvent? MobCommandUsed;

    //Equipment
    public delegate void MobEquip(Mob mob, Item item, Inventory.Slot slot);
    public static MobEquip? MobEquipmentAdded;
    public static MobEquip? MobEquipmentRemoved;

    //Actions
    public delegate void ActionUse(UsageParameters parameters);
    public delegate void MobActionChange(Mob mob, ChessLike.Entity.Action.ActionEvent action);
    public static ActionUse? AbilityUsed;
    public static ObjectChanged<Mob>? MobActionChanged;
    public static MobActionChange? MobActionAdded;
    public static MobActionChange? MobActionRemoved;
    #endregion


    //UI//
    public delegate void ActionEvent(ChessLike.Entity.Action.Ability action);
    public static ActionEvent? ActionSelected;
    public static Event? TurnEnded;

    #region Storage
    public delegate void InventoryItemChange(Inventory inventory, Inventory.Slot slot, Item item);
    public delegate void InventoryError(Inventory inventory, Inventory.Error error);
    public static ObjectChanged<Inventory>? InventoryChanged;
    public static InventoryItemChange? InventoryItemAdded;
    public static InventoryItemChange? InventoryItemRemoved;
    public static InventoryError? InventoryErrored;
    #endregion

}
