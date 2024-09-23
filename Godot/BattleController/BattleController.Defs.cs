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
    //Default meshes
    private readonly Mesh MESH_TARGETING = new CylinderMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(0,0,1, 0.5f)}
    };

    private readonly Mesh MESH_AOE = new SphereMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,0,0)}
    };

    private readonly Mesh MESH_CURSOR = new PrismMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,0,0)},
        Size = new Godot.Vector3(1,2,1),
        
    };

    private readonly Mesh MESH_OTHER = new PrismMesh(){
        Material = new StandardMaterial3D(){AlbedoColor = new(1,1,0)}
    };

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
    public Vector3i PositionSelected;

    //Current turn
    public Action.UsageParams? TurnUsageParameters;

    //Misc
}
