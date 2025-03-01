using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Entity.Action;
using ChessLike.Turn;
using ChessLike.World;
using ChessLike.World.Encounter;
using Godot.Display;

namespace Godot;

public partial class BattleController
{
    //State-machine-related
    public float StateTimeWithoutChange;

    //Misc inputs
    public int InputEndTurnPressed;
    public Ability? InputActionSelected;

    //Components (static)
    public static MobMeshDisplay CompMobMeshDisplay = new();
    public static CombatGeneralUI CompCombatUI = new CombatGeneralUI().GetInstantiatedScene<CombatGeneralUI>();
    public static GridNode CompDisplayGrid = new();
    public static Camera CompCamera = new(){mode = Camera.Mode.DELEGATED_PIVOT};
    public static TurnManager CompTurnManager = new();
    public static Grid CompGrid;
    public static ActionEventRunner CompActionRunner = new();

    //Combat data
    public static EncounterData Encounter;
    public Vector3i PositionHovered;
    public Vector3i PositionSelected = Vector3i.INVALID; //This should always return to INVALID when not in use.
    public int RoundsPassed;

    //Current turn
    public Ability.UsageParameters? TurnUsageParameters;

    //Misc
}
