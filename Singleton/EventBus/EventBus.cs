using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Entity.MobCommand;
using ChessLike.Shared.Storage;
using ChessLike.Turn;
using ChessLike.World;
using ChessLike.World.Encounter;
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
    public delegate void ObjectChange<T>(T obj);
    public delegate void ObjectChangeFrom<TFromTo>(TFromTo old_obj, TFromTo new_obj);
    public delegate void StringEvent(string text);
    public delegate void FloatEvent(float floating);
    public delegate void IntEvent(int integer);

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    //World map
    #region World Map
    public delegate void MapMarkerEvent(MapMarker3D marker);
    public static MapMarkerEvent? MarkerSelected;

    //Save profile
    public static StringEvent? ProfileNameChanged;
    #endregion

    #region Battle Encounter
    //Encounter
    public static ObjectChange<Grid>? GridLoaded;
    public static ObjectChange<EncounterData>? EncounterLoaded;
    public static Event? RoundEnded;
    public static ObjectChange<BattleControllerState>? BattleStateChanged;
    public static ObjectChange<UsageParameters>? SelectedUsageParametersChanged;

    #region TARGETING State
    public static ObjectChange<List<Vector3i>>? TargetPositionsSelected;

    #endregion

    #endregion

    #region Turn
    public delegate void TurnChange(Mob who, bool started);
    public static FloatEvent? TurnTimePassed;
    public static TurnChange? TurnChanged;

    #endregion

    #region Mob
    public delegate void MobEvent(Mob mob);

    #region Mob - Turn
    public static MobEvent? MobTurnStarted;
    public static MobEvent? MobTurnEnded;
    #endregion

    #region Mob - State
    public delegate void MobStateChange(Mob mob, EMobState state);
    public static MobStateChange? MobStateChanged;
    #endregion

    #region Mob - Stats
    public delegate void MobEventStat(Mob mob, EStatName stat, float new_value);
    public static MobEventStat? MobStatChanged;
    #endregion

    #region Mob - Movement
    public delegate void MobMovement(Mob mob, Vector3i from, Vector3i to);
    public delegate void MobMovementPath(Mob mob, List<Vector3i> path);
    public static MobMovement? MobMoved;
    public static MobMovementPath? MobFinishedPathMove;
    #endregion

    #region Mob - Command
    public delegate void MobCommandBroadcast(Dictionary<EInfo, string> dict);
    public delegate void MobCommandEvent(ChessLike.Entity.MobCommand.Command command, Mob onWho);
    public static MobCommandBroadcast? MobCommandBroadcasted;
    public static MobCommandEvent? MobCommandUsed;
    #endregion

    #region Mob - Equipment
    public delegate void MobEquip(Mob mob, Item item, Inventory.Slot slot);
    public static MobEquip? MobEquipmentAdded;
    public static MobEquip? MobEquipmentRemoved;
    #endregion

    #endregion

    #region ActionEvent
    public delegate void ActionUsageParametersEvent(UsageParameters parameters);
    public delegate void MobActionChange(Mob mob, ChessLike.Entity.Action.ActionEvent action);

    #region ActionEvent Queue
    public static ActionUsageParametersEvent? ActionAboutToBeQueued;
    public static ActionUsageParametersEvent? ActionQueued;

    public static ActionUsageParametersEvent? ActionEventQueueRequested;

    public static ActionUsageParametersEvent? ActionAboutToBeUsed;
    public static ActionUsageParametersEvent? ActionUsed;
    #endregion

    #region ActionEvent Mob Action
    public static ObjectChange<Mob>? MobActionChanged;
    public static MobActionChange? MobActionAdded;
    public static MobActionChange? MobActionRemoved;
    #endregion
    #endregion


    #region UI
    public delegate void ActionEvent(ChessLike.Entity.Action.Ability action);
    public static ActionEvent? InputActionSelected;
    public static Event? InputTurnEnded;
    #endregion

    #region Storage
    public delegate void InventoryItemChange(Inventory inventory, Inventory.Slot slot, Item item);
    public delegate void InventoryError(Inventory inventory, Inventory.Error error);
    public static ObjectChange<Inventory>? InventoryChanged;
    public static InventoryItemChange? InventoryItemAdded;
    public static InventoryItemChange? InventoryItemRemoved;
    public static InventoryError? InventoryErrored;
    #endregion


}
