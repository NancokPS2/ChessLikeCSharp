using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessLike.Entity;
using ChessLike.Turn;
using ChessLike.World;
using Godot.Display;
using Action = ChessLike.Entity.Action;

namespace Godot;

public partial class BattleController
{
    //State-machine-related
    private State? StatePrePause;
    public float StateTimeWithoutChange;

    //Misc inputs
    public int InputEndTurnPressed;
    public Action? InputActionSelected;

    //Components
    public MobDisplay CompDisplayMob = new();
    public GridNode CompDisplayGrid = new();
    public Camera CompCamera = new(){mode = Camera.Mode.DELEGATED_PIVOT};
    public TurnManager CompTurnManager = new();
    public Grid CompGrid;
    public Pause CompPauseMenu = new();

    //Combat data
    public EncounterData Encounter;
    public Vector3i PositionHovered;
    public Vector3i PositionSelected = Vector3i.INVALID; //This should always return to INVALID when not in use.

    //Current turn
    public Action.UsageParams? TurnUsageParameters;

    //Misc
}
