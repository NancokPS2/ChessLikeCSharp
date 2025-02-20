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
    public delegate void MobEventStat(Mob mob, StatName stat, float amount);
    public static MobEventStat? MobStatChanged;

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
    public static MobActionChange? MobActionAdded;
    public static MobActionChange? MobActionRemoved;
    #endregion


    //UI//
    public delegate void ActionEvent(ChessLike.Entity.Action.Ability action);
    public static ActionEvent? ActionSelected;
    public static Event? TurnEnded;

    #region Storage
    #endregion

}
