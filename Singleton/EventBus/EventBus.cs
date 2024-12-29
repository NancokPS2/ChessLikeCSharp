using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Command;
using Godot;

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

    //Save profile
    public static StringEvent? ProfileNameChanged;

    //Encounter
    public static Event? RoundEnded;

    //MOBS//
    public delegate void MobEvent(Mob mob);
    public static MobEvent? MobTurnStarted;
    public static MobEvent? MobTurnEnded;

    //Mob stats
    public delegate void MobEventStat(Mob mob, StatName stat, float amount);
    public static MobEventStat? MobStatChanged;

    //Mob commands
    public delegate void MobCommandEvent(Dictionary<EInfo, string> dict);
    public static MobCommandEvent? MobCommandUsed;


    //UI//
    public delegate void ActionEvent(ChessLike.Entity.Action.Ability action);
    public static ActionEvent? ActionSelected;
    public static Event? TurnEndRequested;
}
