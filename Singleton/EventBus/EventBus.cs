using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Entity.MobCommand;
using ChessLike.Storage;
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

    #region Grid
    public delegate void GridPositionsLayered(List<Vector3i> positions, GridNode.Layer layer);
    public delegate void GridCellPositionInput(Vector3i cellPos, ECellInput input);
    public delegate void GridCellPosition(Vector3i cellPos);
    public delegate void GridCellInput(Vector3i cellPos, GridCell cell, ECellInput input);
    public static GridCellPosition? CellPositionSelected;
    public static GridCellPosition? CellPositionHovered;
    public static GridCellInput? CellPositionInputReceived;
    public static GridCellInput? CellInputReceived;
    public static GridPositionsLayered? GridMeshRequested;
    #endregion

    #region Battle Encounter
    //Encounter
    public static ObjectChange<Grid>? GridLoaded;
    public static ObjectChange<EncounterData>? EncounterLoading;

    //Combat start and end
    public static Event? CombatStarted;
    public static Event? CombatEnded;

    //Round start and end
    //This is handled by starting a turn already: public static Event? RoundStarted;
    /// <summary>
    /// Emitted when all current participants have had a turn.
    /// </summary>
    public static Event? RoundEnded;

    public static ObjectChange<EBattleState>? BattleStateChanged;
    public static ObjectChange<UsageParameters>? SelectedUsageParametersChanged;

    #region TARGETING State
    public static ObjectChange<List<Vector3i>>? TargetPositionsSelected;

    #endregion

    #endregion

    #region Turn
    public static FloatEvent? TurnTimePassed;
    public static MobEvent? MobTurnStarted;
    public static MobEvent? MobTurnEnded;
    #endregion

    #region Mob
    public delegate void MobEvent(Mob mob);

    #region Mob - State
    public delegate void MobStateChange(Mob mob, EMobState state);
    public static MobStateChange? MobStateChanged;
    #endregion

    #region Mob - Stats
    public delegate void MobEventStat(Mob mob, EStatName stat, float change);
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
    public delegate void MobEquip(Mob mob, Item item, ItemFilter slot);
    public static MobEquip? MobEquipmentAdded;
    public static MobEquip? MobEquipmentRemoved;
    #endregion

    #endregion

    #region ActionEvent
    public delegate void ActionUsageParametersEvent(UsageParameters parameters);
    public delegate void ActionUsageParametersListEvent(List<UsageParameters> parameterList);
    public delegate void ActionAutoActivation(UsageParameters activated, UsageParameters activatedBy);
    public delegate void MobActionChange(Mob mob, ChessLike.Entity.Action.ActionEvent action);

    #region ActionEvent Queue
    public static ActionUsageParametersEvent? ActionPreQueued;
    public static ActionUsageParametersEvent? ActionQueued;

    /// <summary>
    /// When the <c>ActionEventRunner</c> finishes its Queue
    /// </summary>
    public static ActionUsageParametersListEvent? ActionEventQueueFinished;

    /// <summary>
    /// Invoked by <c>ActionEvent</c>s reacting to another activating. This adds the one activating to the queue
    /// </summary>
    public static ActionAutoActivation? ActionEventAutoActivated;

    //Action usage
    public static ActionUsageParametersEvent? ActionPreUsed;
    public static ActionUsageParametersEvent? ActionUsed;

    public static ActionUsageParametersEvent? ActionAnimationStarted;
    public static ActionUsageParametersEvent? ActionAnimationEnded;
    public static ActionUsageParametersListEvent? ActionAnimationQueueEnded;

    public static ActionUsageParametersEvent? TargetingUsageParametersGenerated;
    public static ActionUsageParametersEvent? TargetingParametersDone;

    #endregion

    #region ActionEvent Mob Action
    public static ObjectChange<Mob>? MobActionChanged;
    public static MobActionChange? MobActionAdded;
    public static MobActionChange? MobActionRemoved;
    #endregion
    #endregion


    #region UI
    public static ObjectChange<ActionEvent>? InputActionSelected;
    public static Event? InputTurnEnded;
    public static Event? InputBack;
    public static Event? InputPause;
    public static ObjectChange<Mob>? MobSelected;
    public static ObjectChange<Mob>? MobHovered;
    #endregion

    #region Storage
    public delegate void InventoryItemChange(MobEquipmentInventory inventory, ItemFilter slot, Item item);
    public static ObjectChange<MobEquipmentInventory>? InventoryChanged;
    public static InventoryItemChange? InventoryItemAdded;
    public static InventoryItemChange? InventoryItemRemoved;
	#endregion



}
